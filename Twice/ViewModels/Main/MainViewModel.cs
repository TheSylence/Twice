using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Squirrel;
using Twice.Messages;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.Services.Views;
using Twice.ViewModels.Columns;
using Twice.ViewModels.Columns.Definitions;
using Twice.Views;

namespace Twice.ViewModels.Main
{
	internal class MainViewModel : ViewModelBaseEx, IMainViewModel
	{
		public MainViewModel( ITwitterContextList contextList, IStatusMuter muter, INotifier notifier, IColumnDefinitionList columnList,
			IConfig config )
		{
			ContextList = contextList;
			ContextList.ContextsChanged += ContextList_ContextsChanged;

			Columns = new ObservableCollection<IColumnViewModel>();
			Notifier = notifier;
			Factory = new ColumnFactory( ContextList, muter, config );
			ColumnList = columnList;
			ColumnList.ColumnsChanged += ColumnList_ColumnsChanged;
			ConstructColumns();
		}

		public async Task OnLoad( object data )
		{
			foreach( var col in Columns )
			{
				await col.Load();
			}

			if( !HasContexts )
			{
				var csa = new ConfirmServiceArgs( Strings.DoYouWantToAddANewAccount, Strings.NoAccountAdded );

				if( await ViewServiceRepository.Confirm( csa ) )
				{
					await ViewServiceRepository.ShowAccounts( true );
				}
			}

			if( Configuration.General.CheckForUpdates )
			{
				var channelUrl = Configuration.General.IncludePrereleaseUpdates ? Constants.IO.BetaChannelUrl : Constants.IO.ReleaseChannelUrl;

				try
				{
					using( var mgr = new UpdateManager( channelUrl ) )
					{
						await mgr.UpdateApp();
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
			}
			Columns.Clear();

			var definitions = ColumnList.Load();
			var constructed = definitions.Select( c => Factory.Construct( c ) );
			constructed = constructed.Where( c => c != null );

			foreach( var c in constructed )
			{
				c.NewStatus += Col_NewStatus;
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
		public bool HasContexts => ContextList.Contexts.Any();
		public ICommand InfoCommand => _InfoCommand ?? ( _InfoCommand = new RelayCommand( ExecuteInfoCommand ) );
		public ICommand NewTweetCommand => _NewTweetCommand ?? ( _NewTweetCommand = new RelayCommand( ExecuteNewTweetCommand, CanExecuteNewTweetCommand ) );
		public ICommand SettingsCommand => _SettingsCommand ?? ( _SettingsCommand = new RelayCommand( ExecuteSettingsCommand ) );
		private readonly IColumnDefinitionList ColumnList;
		private readonly ColumnFactory Factory;
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