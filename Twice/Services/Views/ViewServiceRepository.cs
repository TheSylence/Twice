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
using Microsoft.Win32;
using Twice.Models.Columns;
using Twice.Models.Twitter;
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
		public async Task ComposeTweet()
		{
			await ShowWindow<TweetComposer, IComposeTweetViewModel>();
		}

		public async Task<bool> Confirm( ConfirmServiceArgs args )
		{
			Action<IConfirmDialogViewModel> vmSetup = vm =>
			{
				vm.Title = args.Title;
				vm.Label = args.Message;
			};
			Func<IConfirmDialogViewModel, object> resultSetup = vm => true;

			var result = await ShowWindow<ConfirmDialog, IConfirmDialogViewModel, object>( resultSetup, vmSetup );

			return result != null;
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

		public async Task QuoteTweet( StatusViewModel status, IEnumerable<ulong> preSelectedAccounts = null )
		{
			Action<IComposeTweetViewModel> vmSetup = vm =>
			{
				vm.QuotedTweet = status;
				vm.PreSelectAccounts( preSelectedAccounts ?? new ulong[0] );
			};

			await ShowWindow<TweetComposer, IComposeTweetViewModel>( vmSetup );
		}

		public async Task ReplyToTweet( StatusViewModel status, bool toAll )
		{
			Action<IComposeTweetViewModel> vmSetup = vm => { vm.SetReply( status, toAll ); };

			await ShowWindow<TweetComposer, IComposeTweetViewModel>( vmSetup );
		}

		public async Task RetweetStatus( StatusViewModel status )
		{
			Action<IRetweetDialogViewModel> vmSetup = vm => { vm.Status = status; };

			await ShowWindow<RetweetDialog, IRetweetDialogViewModel>( vmSetup );
		}

		public async Task<ColumnDefinition[]> SelectAccountColumnTypes( ulong accountId )
		{
			ulong[] sourceAccounts = {accountId};
			ulong[] targetAccounts = {accountId};

			Func<IColumnTypeSelectionDialogViewModel, ColumnDefinition[]> resultSetup = vm =>
			{
				return vm.AvailableColumnTypes.Where( c => c.IsSelected ).Select( c => c.Content.Type )
					.Select( type => ColumnDefinitionFactory.Construct( type, sourceAccounts, targetAccounts ) ).ToArray();
			};

			return await ShowWindow<AccountColumnsDialog, IColumnTypeSelectionDialogViewModel, ColumnDefinition[]>( resultSetup );
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

		public string TextInput( string label, string input = null )
		{
			Func<ITextInputDialogViewModel, string> resultSetup = vm => vm.Input;
			Action<ITextInputDialogViewModel> vmSetup = vm =>
			{
				vm.Label = label;
				vm.Input = input ?? string.Empty;
				vm.ClearValidationErrors();
			};

			return ShowWindowSync<TextInputDialog, ITextInputDialogViewModel, string>( resultSetup, vmSetup );
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

		public async Task ViewStatus( StatusViewModel status, IContextEntry context )
		{
			Action<ITweetDetailsViewModel> vmSetup = vm =>
			{
				vm.Context = context;
				vm.DisplayTweet = status;
			};

			await ShowWindow<TweetDetailsDialog, ITweetDetailsViewModel>( vmSetup );
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

		private TResult ShowWindowSync<TWindow, TViewModel, TResult>( Func<TViewModel, TResult> resultSetup = null,
			Action<TViewModel> vmSetup = null )
			where TViewModel : class
			where TResult : class
			where TWindow : Window, new()
		{
			ManualResetEventSlim waitHandle = new ManualResetEventSlim( false );

			TResult result = null;

			DispatcherHelper.CheckBeginInvokeOnUI(
				async () =>
				{
					result = await ShowWindow<TWindow, TViewModel, TResult>( resultSetup, vmSetup );
					waitHandle.Set();
				} );

			waitHandle.Wait();
			return result;
		}

		public Dialog CurrentDialog { get; }

		private static MetroWindow Window
			=> Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault( x => x.IsActive );
	}
}