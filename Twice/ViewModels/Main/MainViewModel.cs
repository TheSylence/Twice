using Anotar.NLog;
using Fody;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Twice.Messages;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.Utilities;
using Twice.Utilities.Ui;
using Twice.ViewModels.Columns;
using Twice.Views.Services;

namespace Twice.ViewModels.Main
{
	// ReSharper disable once ClassNeverInstantiated.Global
	[ConfigureAwait( false )]
	internal class MainViewModel : ViewModelBaseEx, IMainViewModel
	{
		public MainViewModel( ITwitterContextList contextList, INotifier notifier, IColumnDefinitionList columnList,
			IColumnFactory columnFactory,
			IMessenger messenger = null )
			: base( messenger )
		{
			ContextList = contextList;
			ContextList.ContextsChanged += ContextList_ContextsChanged;

			Columns = new ObservableCollection<IColumnViewModel>();
			Notifier = notifier;
			Factory = columnFactory;
			ColumnList = columnList;
			ColumnList.ColumnsChanged += ColumnList_ColumnsChanged;
			ConstructColumns();

			DragDropHandler = new DragDropHandler( columnList, MessengerInstance );
			var rateLimitTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMinutes( 15 )
			};
			rateLimitTimer.Tick += RateLimitTimer_Tick;
			rateLimitTimer.Start();

			var statusUpdateTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds( 10 )
			};
			statusUpdateTimer.Tick += StatusUpdateTimer_Tick;
			statusUpdateTimer.Start();

			var updateCheckTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromHours( 1 )
			};
			updateCheckTimer.Tick += UpdateCheckTimer_Tick;
			updateCheckTimer.Start();
		}

		[SuppressMessage( "ReSharper", "NotAccessedVariable" )]
		public async Task OnLoad( object data )
		{
			if( !HasContexts )
			{
				var csa = new ConfirmServiceArgs( Strings.DoYouWantToAddANewAccount, Strings.NoAccountAdded );

				if( await ViewServiceRepository.Confirm( csa ) )
				{
					await ViewServiceRepository.ShowAccounts( true );
				}
			}

			Task.Run( async () =>
			{
				await CheckCredentials();
				await ReportAppVersion();
			} ).Forget();
			
			await Task.WhenAll( Columns.Select( c => c.Load( AsyncLoadContext.Ui ) ) );

			// It's late and I didn't have enough coffee...
			ColumnsLocked = !Configuration.General.ColumnsLocked;
			await Dispatcher.RunAsync( ExecuteToggleColumnsLockCommand );

			try
			{
				await TwitterConfig.QueryConfig();
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to read current config from twitter", ex );
			}

			await Task.WhenAll( CheckForUpdates(), QueryRateLimit() );
		}

		private bool CanExecuteAddColumnCommand()
		{
			return HasContexts;
		}

		private bool CanExecuteNewMessageCommand()
		{
			return HasContexts;
		}

		private bool CanExecuteNewTweetCommand()
		{
			return HasContexts;
		}

		private async Task CheckCredentials()
		{
			foreach( var context in ContextList.Contexts )
			{
				try
				{
					bool valid = await context.Twitter.VerifyCredentials();
					LogTo.Info( $"Credentials valid for {context.AccountName}: {valid}" );
				}
				catch( Exception ex )
				{
					LogTo.WarnException( $"Credentials for {context.AccountName} could not be checked", ex );
				}
			}
		}

		private async Task CheckForUpdates()
		{
			if( Configuration?.General?.CheckForUpdates == true )
			{
				bool useBetaChannel = Configuration?.General?.IncludePrereleaseUpdates == true;

				LogTo.Info( "Searching for app updates..." );
				LogTo.Info( $"Using beta channel for updates: {useBetaChannel}" );

				try
				{
					using( var mgr = await UpdateFactory.Construct( useBetaChannel ) )
					{
						var release = await mgr.UpdateApp();

						Version newVersion = release?.Version?.Version;

						if( newVersion == null )
						{
							LogTo.Warn( "UpdateApp returned null" );
						}
						else if( newVersion > Assembly.GetExecutingAssembly().GetName().Version )
						{
							LogTo.Info( $"Updated app to {release.Version}" );
							Notifier.DisplayMessage( string.Format( Strings.UpdateHasBeenInstalled, release.Version ),
								NotificationType.Information );
						}
						else
						{
							LogTo.Info( "App is up to date" );
						}
					}
				}
				catch( Exception ex ) when( ex.Message.Contains( "Update.exe" ) )
				{
				}
				catch( Exception ex )
				{
					LogTo.WarnException( "Error during update check", ex );
				}
			}
		}

		private void Col_Changed( object sender, EventArgs e )
		{
			var col = sender as IColumnViewModel;
			Debug.Assert( col != null, "col != null" );

			var def = col.Definition;

			var definitions = ColumnList.Load().ToArray();

			var updated = definitions.First( d => d.Id == def.Id );
			updated.Width = def.Width;
			updated.Notifications = def.Notifications;

			ColumnList.Update( definitions );
		}

		private void Col_Deleted( object sender, EventArgs e )
		{
			var col = sender as IColumnViewModel;
			Debug.Assert( col != null, "col != null" );

			ColumnList.Remove( new[] { col.Definition } );
		}

		private void Col_NewItem( object sender, ColumnItemEventArgs e )
		{
			var vm = sender as IColumnViewModel;
			Debug.Assert( vm != null );

			ColumnNotifications columnSettings = vm.Definition.Notifications;
			Notifier.OnItem( e.Item, columnSettings );
		}

		private async void ColumnList_ColumnsChanged( object sender, EventArgs e )
		{
			ConstructColumns();
			await OnLoad( null );
		}

		private void ConstructColumns()
		{
			foreach( var c in Columns )
			{
				c.NewItem -= Col_NewItem;
				c.Changed -= Col_Changed;
				c.Deleted -= Col_Deleted;
			}

			Columns.Clear();

			var definitions = ColumnList.Load();
			var constructed = definitions.Select( c => Factory.Construct( c ) );
			constructed = constructed.Where( c => c != null );

			foreach( var c in constructed )
			{
				c.NewItem += Col_NewItem;
				c.Changed += Col_Changed;
				c.Deleted += Col_Deleted;
				Columns.Add( c );
			}
		}

		private void ContextList_ContextsChanged( object sender, EventArgs e )
		{
			ColumnList.SetExistingContexts( ContextList.Contexts.Select( c => c.UserId ) );

			RaisePropertyChanged( nameof( HasContexts ) );
		}

		private async void ExecuteAccountsCommand()
		{
			await ViewServiceRepository.ShowAccounts();
		}

		private async void ExecuteAddColumnCommand()
		{
			await ViewServiceRepository.ShowAddColumnDialog();
		}

		private async void ExecuteInfoCommand()
		{
			await ViewServiceRepository.ShowInfo();
		}

		private async void ExecuteNewMessageCommand()
		{
			await ViewServiceRepository.ComposeMessage();
		}

		private async void ExecuteNewTweetCommand()
		{
			await ViewServiceRepository.ComposeTweet();
		}

		private async void ExecuteSearchCommand()
		{
			await ViewServiceRepository.OpenSearch();
		}

		private async void ExecuteSettingsCommand()
		{
			await ViewServiceRepository.ShowSettings();
		}

		private void ExecuteToggleColumnsLockCommand()
		{
			ColumnsLocked = !ColumnsLocked;
			MessengerInstance.Send( new ColumnLockMessage( ColumnsLocked ) );
		}

		private async Task QueryRateLimit()
		{
			foreach( var context in ContextList.Contexts )
			{
				try
				{
					await context.Twitter.LogCurrentRateLimits();
				}
				catch( Exception ex )
				{
					LogTo.WarnException( $"Could not query rate limit for {context.AccountName}", ex );
				}
			}
		}

		private async void RateLimitTimer_Tick( object sender, EventArgs e )
		{
			await QueryRateLimit();
		}

		private Task ReportAppVersion()
		{
			if( !Configuration.General.SendVersionStats )
			{
				return Task.CompletedTask;
			}

			try
			{
				return VersionReporter.Report();
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to report version info", ex );
				return Task.CompletedTask;
			}
		}

		private void StatusUpdateTimer_Tick( object sender, EventArgs e )
		{
			foreach( var col in Columns )
			{
				col.UpdateRelativeTimes();
			}
		}

		private async void UpdateCheckTimer_Tick( object sender, EventArgs e )
		{
			await CheckForUpdates();
		}

		public ICommand AccountsCommand
			=> _AccountsCommand ?? ( _AccountsCommand = new RelayCommand( ExecuteAccountsCommand ) );

		public ICommand AddColumnCommand
			=>
			_ManageColumnsCommand
			?? ( _ManageColumnsCommand = new RelayCommand( ExecuteAddColumnCommand, CanExecuteAddColumnCommand ) );

		public ICollection<IColumnViewModel> Columns { get; }

		public bool ColumnsLocked
		{
			[DebuggerStepThrough]
			get { return _ColumnsLocked; }
			set
			{
				if( _ColumnsLocked == value )
				{
					return;
				}

				_ColumnsLocked = value;
				RaisePropertyChanged();
			}
		}

		[Inject]
		public IDispatcher Dispatcher { get; set; }

		public IDragDropHandler DragDropHandler { get; }

		public bool HasContexts => ContextList.Contexts.Any();

		public ICommand InfoCommand => _InfoCommand ?? ( _InfoCommand = new RelayCommand( ExecuteInfoCommand ) );

		public ICommand NewMessageCommand
			=>
			_NewMessageCommand
			?? ( _NewMessageCommand = new RelayCommand( ExecuteNewMessageCommand, CanExecuteNewMessageCommand ) );

		public ICommand NewTweetCommand
			=> _NewTweetCommand ?? ( _NewTweetCommand = new RelayCommand( ExecuteNewTweetCommand, CanExecuteNewTweetCommand ) );

		public ICommand SearchCommand => _SearchCommand ?? ( _SearchCommand = new RelayCommand(
											 ExecuteSearchCommand ) );

		public ICommand SettingsCommand
			=> _SettingsCommand ?? ( _SettingsCommand = new RelayCommand( ExecuteSettingsCommand ) );

		public ICommand ToggleColumnsLockCommand
			=> _ToggleColumnsLockCommand ?? ( _ToggleColumnsLockCommand = new RelayCommand( ExecuteToggleColumnsLockCommand ) );

		[Inject]
		public IAppUpdaterFactory UpdateFactory { private get; set; }

		private readonly IColumnDefinitionList ColumnList;

		private readonly IColumnFactory Factory;

		private readonly INotifier Notifier;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _AccountsCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _ColumnsLocked;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _InfoCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _ManageColumnsCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _NewMessageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _NewTweetCommand;

		private RelayCommand _SearchCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _SettingsCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _ToggleColumnsLockCommand;
	}
}