using Anotar.NLog;
using GalaSoft.MvvmLight.CommandWpf;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Twice.Messages;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.Services.Views;
using Twice.ViewModels.Columns;
using Twice.Views;

namespace Twice.ViewModels.Main
{
	// ReSharper disable once ClassNeverInstantiated.Global
	internal class MainViewModel : ViewModelBaseEx, IMainViewModel
	{
		public MainViewModel( ITwitterContextList contextList, INotifier notifier, IColumnDefinitionList columnList, IColumnFactory columnFactory )
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
		}

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

			foreach( var col in Columns )
			{
				await col.Load();
			}

			if( Configuration?.General?.CheckForUpdates == true )
			{
				bool useBetaChannel = Configuration?.General?.IncludePrereleaseUpdates == true;

				var channelUrl = useBetaChannel
					? Constants.Updates.BetaChannelUrl
					: Constants.Updates.ReleaseChannelUrl;

				LogTo.Info( $"Using beta channel for updates: {useBetaChannel}" );

				try
				{
					using( var mgr = new UpdateManager( channelUrl ) )
					{
						var release = await mgr.UpdateApp();

						Version newVersion = release?.Version?.Version;

						if( newVersion == null )
						{
							LogTo.Warn( "UpdateApp returned null" );
						}
						else if( newVersion > Assembly.GetExecutingAssembly().GetName().Version )
						{
							Notifier.DisplayMessage( string.Format( Strings.UpdateHasBeenInstalled, release.Version ),
								NotificationType.Information );
						}
					}
				}
				catch( Exception ex ) when( ex.Message.Contains( "Update.exe" ) )
				{
				}
			}
		}

		private bool CanExecuteAddColumnCommand()
		{
			return HasContexts;
		}

		private bool CanExecuteNewTweetCommand()
		{
			return HasContexts;
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

		private void Col_NewStatus( object sender, StatusEventArgs e )
		{
			var vm = sender as IColumnViewModel;
			Debug.Assert( vm != null );

			ColumnNotifications columnSettings = vm.Definition.Notifications;
			Notifier.OnStatus( e.Status, columnSettings );
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
				c.NewStatus -= Col_NewStatus;
				c.Changed -= Col_Changed;
				c.Deleted -= Col_Deleted;
			}
			Columns.Clear();

			var definitions = ColumnList.Load();
			var constructed = definitions.Select( c => Factory.Construct( c ) );
			constructed = constructed.Where( c => c != null );

			foreach( var c in constructed )
			{
				c.NewStatus += Col_NewStatus;
				c.Changed += Col_Changed;
				c.Deleted += Col_Deleted;
				Columns.Add( c );
			}
		}

		private void ContextList_ContextsChanged( object sender, EventArgs e )
		{
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

		private void ExecuteNewTweetCommand()
		{
			MessengerInstance.Send( new FlyoutMessage( FlyoutNames.TweetComposer, FlyoutAction.Open ) );
		}

		private async void ExecuteSettingsCommand()
		{
			await ViewServiceRepository.ShowSettings();
		}

		public ICommand AccountsCommand => _AccountsCommand ?? ( _AccountsCommand = new RelayCommand( ExecuteAccountsCommand ) );

		public ICommand AddColumnCommand
			=> _ManageColumnsCommand ?? ( _ManageColumnsCommand = new RelayCommand( ExecuteAddColumnCommand, CanExecuteAddColumnCommand ) );

		public ICollection<IColumnViewModel> Columns { get; }
		public IDragDropHandler DragDropHandler { get; }
		public bool HasContexts => ContextList.Contexts.Any();
		public ICommand InfoCommand => _InfoCommand ?? ( _InfoCommand = new RelayCommand( ExecuteInfoCommand ) );
		public ICommand NewTweetCommand => _NewTweetCommand ?? ( _NewTweetCommand = new RelayCommand( ExecuteNewTweetCommand, CanExecuteNewTweetCommand ) );
		public ICommand SettingsCommand => _SettingsCommand ?? ( _SettingsCommand = new RelayCommand( ExecuteSettingsCommand ) );
		private readonly IColumnDefinitionList ColumnList;
		private readonly IColumnFactory Factory;
		private readonly INotifier Notifier;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _AccountsCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _InfoCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _ManageColumnsCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _NewTweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _SettingsCommand;
	}
}