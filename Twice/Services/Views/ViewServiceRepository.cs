using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Twice.Resources;
using Twice.ViewModels.Accounts;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Info;
using Twice.ViewModels.Profile;
using Twice.Views;
using Twice.Views.Dialogs;
using AccountsDialog = Twice.Views.Dialogs.AccountsDialog;
using AddColumnDialog = Twice.Views.Wizards.AddColumn.AddColumnDialog;
using InfoDialog = Twice.Views.Dialogs.InfoDialog;
using ProfileDialog = Twice.Views.Dialogs.ProfileDialog;
using SettingsDialog = Twice.Views.Dialogs.SettingsDialog;

namespace Twice.Services.Views
{
	internal class ViewServiceRepository : IViewServiceRepository
	{
		public async Task<bool> Confirm( ConfirmServiceArgs args )
		{
			Debug.Assert( args != null );

			var settings = new MetroDialogSettings
			{
				AffirmativeButtonText = Strings.Yes,
				NegativeButtonText = Strings.No
			};
			var result = await Window.ShowMessageAsync( args.Title, args.Message, MessageDialogStyle.AffirmativeAndNegative, settings );

			return result == MessageDialogResult.Affirmative;
		}

		public Task<string> OpenFile( FileServiceArgs fsa = null )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if( fsa != null )
			{
				dlg.Filter = fsa.Filter;
			}

			if( dlg.ShowDialog( Application.Current.MainWindow ) == true )
			{
				return Task.FromResult( dlg.FileName );
			}

			return Task.FromResult<string>( null );
		}

		public async Task ShowAccounts()
		{
			await ShowDialog<AccountsDialog, IAccountsDialogViewModel, object>();
		}

		public async Task ShowAddColumnDialog()
		{
			await ShowWindow<AddColumnDialog, IAddColumnDialogViewModel>();
		}

		public async Task ShowInfo()
		{
			await ShowWindow<InfoDialog, IInfoDialogViewModel>();
		}

		public Task ShowSettings()
		{
			var dlg = new SettingsDialog
			{
				Owner = Window
			};

			dlg.ShowDialog();

			return Task.CompletedTask;
		}

		public async Task ViewProfile( ulong userId )
		{
			Action<IProfileDialogViewModel> vmSetup = vm =>
			{
				vm.Setup( userId );
			};

			await ShowWindow<ProfileDialog, IProfileDialogViewModel, object>( null, vmSetup );
		}

		private void CloseHandler( object sender, DialogClosingEventArgs eventargs )
		{
			CurrentDialog = null;
		}

		private void OpenHandler( object sender, DialogOpenedEventArgs eventargs )
		{
			CurrentDialog = sender as Dialog;
		}

		private async Task<TResult> ShowDialog<TDialog, TViewModel, TResult>( Func<TViewModel, TResult> resultSetup = null, Action<TViewModel> vmSetup = null )
							where TDialog : Dialog, new()
			where TViewModel : class
			where TResult : class
		{
			var dlg = new TDialog();
			var vm = dlg.DataContext as TViewModel;
			Debug.Assert( vm != null );

			vmSetup?.Invoke( vm );

			var result = await DialogHost.Show( dlg, OpenHandler, CloseHandler ) as bool?;
			if( result != true )
			{
				return null;
			}

			Func<TViewModel, TResult> defaultResultSetup = _ => default( TResult );
			var resSetup = resultSetup ?? defaultResultSetup;
			return resSetup( vm );
		}

		private async Task ShowWindow<TWindow, TViewModel>( Action<TViewModel> vmSetup = null )
			where TViewModel : class
			where TWindow : Window, new()
		{
			await ShowWindow<TWindow, TViewModel, object>( null, vmSetup );
		}

		private Task<TResult> ShowWindow<TWindow, TViewModel, TResult>( Func<TViewModel, TResult> resultSetup = null,
					Action<TViewModel> vmSetup = null )
			where TViewModel : class
			where TResult : class
			where TWindow : Window, new()
		{
			var dlg = new TWindow
			{
				Owner = Window
			};

			var vm = dlg.DataContext as TViewModel;
			Debug.Assert( vm != null );

			vmSetup?.Invoke( vm );

			TResult result = null;

			if( dlg.ShowDialog() == true )
			{
				Func<TViewModel, TResult> defaultResultSetup = _ => default( TResult );
				var resSetup = resultSetup ?? defaultResultSetup;
				result = resSetup( vm );
			}

			return Task.FromResult( result );
		}

		public Dialog CurrentDialog { get; private set; }
		private static MetroWindow Window => Application.Current.MainWindow as MetroWindow;
	}
}