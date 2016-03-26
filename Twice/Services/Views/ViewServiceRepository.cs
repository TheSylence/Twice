using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Ninject;
using Twice.Resources;
using Twice.ViewModels.Accounts;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Settings;
using Twice.Views;

namespace Twice.Services.Views
{
	internal class ViewServiceRepository : IViewServiceRepository
	{
		public ViewServiceRepository()
		{
			Kernel = App.Kernel;
		}

		public async Task ViewProfile( ulong userId )
		{
			Action<IProfileDialogViewModel> vmSetup = vm =>
			{
				vm.Setup( userId );
			};

			await ShowDialog<ProfileDialog, IProfileDialogViewModel, object>( null, vmSetup );
		}

		public async Task<bool> Confirm( ConfirmServiceArgs args )
		{
			ConfirmServiceArgs csa = args as ConfirmServiceArgs;
			Debug.Assert( csa != null );

			var settings = new MetroDialogSettings
			{
				AffirmativeButtonText = Strings.Yes,
				NegativeButtonText = Strings.No
			};
			var result = await Window.ShowMessageAsync( csa.Title, csa.Message, MessageDialogStyle.AffirmativeAndNegative, settings );

			return result == MessageDialogResult.Affirmative;
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

		public async Task ShowAccounts()
		{
			await ShowDialog<AccountsDialog, IAccountsDialogViewModel, object>();
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

			Func<TViewModel, TResult> defaultResultSetup = _ => default(TResult);
			var resSetup = resultSetup ?? defaultResultSetup;
			return resSetup( vm );
		}

		public Dialog CurrentDialog { get; private set; }
		private static MetroWindow Window => Application.Current.MainWindow as MetroWindow;
		private readonly IKernel Kernel;
	}
}