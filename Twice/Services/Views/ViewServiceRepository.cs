using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Fody;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels.Accounts;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Dialogs;
using Twice.ViewModels.Info;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Twitter;
using Twice.Views;
using Twice.Views.Dialogs;
using Twice.Views.Wizards;

namespace Twice.Services.Views
{
	[ExcludeFromCodeCoverage]
	[ConfigureAwait( false )]
	internal class ViewServiceRepository : IViewServiceRepository
	{
		private void CloseHandler( object sender, DialogClosingEventArgs eventargs )
		{
			CurrentDialog = null;
		}

		private void OpenHandler( object sender, DialogOpenedEventArgs eventargs )
		{
			CurrentDialog = sender as Dialog;
		}

		private async Task<TResult> ShowDialog<TDialog, TViewModel, TResult>( Func<TViewModel, TResult> resultSetup = null,
			Action<TViewModel> vmSetup = null,
			string hostIdentifier = null )
			where TDialog : Dialog, new()
			where TViewModel : class
			where TResult : class
		{
			var dlg = new TDialog();
			var vm = dlg.DataContext as TViewModel;
			Debug.Assert( vm != null );

			vmSetup?.Invoke( vm );

			var result = await DialogHost.Show( dlg, hostIdentifier, OpenHandler, CloseHandler ) as bool?;
			if( result != true )
			{
				return null;
			}

			Func<TViewModel, TResult> defaultResultSetup = _ => default(TResult);
			var resSetup = resultSetup ?? defaultResultSetup;
			return resSetup( vm );
		}

		private TResult ShowDialogSync<TDialog, TViewModel, TResult>( Func<TViewModel, TResult> resultSetup = null,
			Action<TViewModel> vmSetup = null,
			string hostIdentifier = null )
			where TDialog : Dialog, new()
			where TViewModel : class
			where TResult : class
		{
			ManualResetEvent waitHandle = new ManualResetEvent( false );

			TResult result = null;
			DispatcherHelper.CheckBeginInvokeOnUI( async () =>
			{
				result = await ShowDialog<TDialog, TViewModel, TResult>( resultSetup, vmSetup, hostIdentifier );
				waitHandle.Set();
			} );

			waitHandle.WaitOne();
			return result;
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
				Func<TViewModel, TResult> defaultResultSetup = _ => default(TResult);
				var resSetup = resultSetup ?? defaultResultSetup;
				result = resSetup( vm );
			}

			return Task.FromResult( result );
		}

		public async Task ComposeTweet()
		{
			await ShowWindow<TweetComposer, IComposeTweetViewModel>();
		}

		public async Task<bool> Confirm( ConfirmServiceArgs args )
		{
			Debug.Assert( args != null );

			var dictionary = new ResourceDictionary
			{
				Source =
					new Uri(
						"pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml" )
			};

			var settings = new MetroDialogSettings
			{
				AffirmativeButtonText = Strings.Yes,
				NegativeButtonText = Strings.No,
				SuppressDefaultResources = true,
				CustomResourceDictionary = dictionary
			};
			var result =
				await Window.ShowMessageAsync( args.Title, args.Message, MessageDialogStyle.AffirmativeAndNegative, settings );

			return result == MessageDialogResult.Affirmative;
		}

		public Task<string> OpenFile( FileServiceArgs fsa = null )
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if( fsa != null )
			{
				dlg.Filter = fsa.Filter;
			}

			return dlg.ShowDialog( Application.Current.MainWindow ) == true
				? Task.FromResult( dlg.FileName )
				: Task.FromResult<string>( null );
		}

		public async Task<ColumnDefinition[]> SelectAccountColumnTypes( ulong accountId, string hostIdentifier )
		{
			ulong[] sourceAccounts = {accountId};
			ulong[] targetAccounts = {accountId};

			Func<IColumnTypeSelectionDialogViewModel, ColumnDefinition[]> resultSetup = vm =>
			{
				return vm.AvailableColumnTypes.Where( c => c.IsSelected ).Select( c => c.Content.Type )
					.Select( type => ColumnDefinitionFactory.Construct( type, sourceAccounts, targetAccounts ) ).ToArray();
			};

			return
				await
					ShowDialog<AccountColumnsDialog, IColumnTypeSelectionDialogViewModel, ColumnDefinition[]>( resultSetup, null,
						hostIdentifier );
		}

		public async Task ShowAccounts( bool directlyAddAccount = false )
		{
			Action<IAccountsDialogViewModel> vmSetup = vm =>
			{
				if( directlyAddAccount )
				{
					vm.AddAccountCommand.Execute( null );
				}
			};
			await ShowWindow<AccountsDialog, IAccountsDialogViewModel, object>( null, vmSetup );
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

		public string TextInput( string label, string input = null, string hostIdentifier = null )
		{
			Func<ITextInputDialogViewModel, string> resultSetup = vm => vm.Input;
			Action<ITextInputDialogViewModel> vmSetup = vm =>
			{
				vm.Label = label;
				vm.Input = input ?? string.Empty;
			};

			return ShowDialogSync<TextInputDialog, ITextInputDialogViewModel, string>( resultSetup, vmSetup, hostIdentifier );
		}

		public async Task ViewImage( IList<Uri> imageSet, Uri selectedImage )
		{
			Action<IImageDialogViewModel> setup = vm =>
			{
				vm.SetImages( imageSet );
				vm.SelectedImage = vm.Images.FirstOrDefault( img => img.ImageUrl == selectedImage )
									?? vm.Images.FirstOrDefault();
			};

			await ShowWindow<ImageDialog, IImageDialogViewModel, object>( null, setup );
		}

		public async Task ViewProfile( ulong userId )
		{
			Action<IProfileDialogViewModel> vmSetup = vm => { vm.Setup( userId ); };

			await ShowWindow<ProfileDialog, IProfileDialogViewModel, object>( null, vmSetup );
		}

		public Task ViewStatus( StatusViewModel vm )
		{
			// TODO: Implement
			return Task.CompletedTask;
		}

		public Dialog CurrentDialog { get; private set; }

		private static MetroWindow Window
			=> Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault( x => x.IsActive );
	}
}