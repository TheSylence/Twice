using Fody;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Twice.Models.Columns;
using Twice.Utilities.Ui;
using Twice.ViewModels.Accounts;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Dialogs;
using Twice.ViewModels.Dialogs.Data;
using Twice.ViewModels.Flyouts;
using Twice.ViewModels.Info;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Twitter;
using Twice.Views.Dialogs;
using Twice.Views.Flyouts;
using Twice.Views.Wizards;
using ColumnDefinition = Twice.Models.Columns.ColumnDefinition;

namespace Twice.Views.Services
{
	[ExcludeFromCodeCoverage]
	[ConfigureAwait( false )]
	internal class ViewServiceRepository : IViewServiceRepository
	{
		public ViewServiceRepository( IDialogStack dialogStack )
		{
			DialogStack = dialogStack;
		}

		public async Task ComposeMessage()
		{
			if( !DialogStack.Push( new ComposeMessageData() ) )
			{
				return;
			}

			await ShowHostedDialog<MessageDialog, IComposeMessageViewModel>();
		}

		public async Task ComposeTweet( string text = null )
		{
			if( !DialogStack.Push( new ComposeTweetData( null, false, text ) ) )
			{
				return;
			}

			await ShowHostedDialog<TweetComposer, IComposeTweetViewModel>();
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
			var dlg = new OpenFileDialog();
			if( fsa != null )
			{
				dlg.Filter = fsa.Filter;
			}

			return dlg.ShowDialog( Application.Current.MainWindow ) == true
				? Task.FromResult( dlg.FileName )
				: Task.FromResult<string>( null );
		}

		public void OpenNotificationFlyout( NotificationViewModel vm )
		{
			Dispatcher.CheckBeginInvokeOnUI( () =>
			{
				if( CurrentlyOpenNotification != null )
				{
					CurrentlyOpenNotification.IsOpen = false;
					( (NotificationViewModel)CurrentlyOpenNotification.DataContext ).OnClosed();
					CurrentlyOpenNotification = null;
				}

				var mainWindow = Application.Current.MainWindow as MetroWindow;
				Flyout flyout = mainWindow?.Flyouts.Items.OfType<NotificationBar>().FirstOrDefault();
				if( flyout == null )
				{
					return;
				}

				flyout.DataContext = vm;
				vm.Reset();

				flyout.IsOpen = true;
				CurrentlyOpenNotification = flyout;
			} );
		}

		public async Task OpenSearch( string query = null )
		{
			if( !DialogStack.Push( new SearchDialogData( query ) ) )
			{
				return;
			}

			await ShowHostedDialog<SearchDialog, ISearchDialogViewModel, object>();
		}

		public async Task QuoteTweet( StatusViewModel status, IEnumerable<ulong> preSelectedAccounts = null )
		{
			if( !DialogStack.Push( new QuoteTweetData( status, preSelectedAccounts?.ToArray() ?? new ulong[0] ) ) )
			{
				return;
			}

			await ShowHostedDialog<TweetComposer, IComposeTweetViewModel>();
		}

		public async Task ReplyToMessage( MessageViewModel message )
		{
			if( !DialogStack.Push( new ComposeMessageData( message.Partner.ScreenName, message ) ) )
			{
				return;
			}

			await ShowHostedDialog<MessageDialog, IComposeMessageViewModel>();
		}

		public async Task ReplyToTweet( StatusViewModel status, bool toAll )
		{
			if( !DialogStack.Push( new ComposeTweetData( status, toAll ) ) )
			{
				return;
			}

			await ShowHostedDialog<TweetComposer, IComposeTweetViewModel>();
		}

		public async Task RetweetStatus( StatusViewModel status )
		{
			Action<IRetweetDialogViewModel> vmSetup = vm => { vm.Status = status; };

			await ShowWindow<RetweetDialog, IRetweetDialogViewModel>( vmSetup );
		}

		public async Task<ColumnDefinition[]> SelectAccountColumnTypes( ulong accountId )
		{
			ulong[] sourceAccounts = { accountId };
			ulong[] targetAccounts = { accountId };

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

		public async Task ViewDirectMessage( MessageViewModel msg )
		{
			if( !DialogStack.Push( new MessageData( msg ) ) )
			{
				return;
			}

			await ShowHostedDialog<MessageDetailsDialog, IMessageDetailsViewModel>();
		}

		public async Task ViewImage( IList<StatusMediaViewModel> imageSet, StatusMediaViewModel selectedImage )
		{
			if( !DialogStack.Push( new ImageData( imageSet, selectedImage ) ) )
			{
				return;
			}

			await ShowHostedDialog<ImageDialog, IImageDialogViewModel>();
		}

		public async Task ViewImage( IList<Uri> imageSet, Uri selectedImage )
		{
			if( !DialogStack.Push( new ImageData( imageSet, selectedImage ) ) )
			{
				return;
			}

			await ShowHostedDialog<ImageDialog, IImageDialogViewModel>();
		}

		public async Task ViewProfile( ulong userId )
		{
			if( !DialogStack.Push( new ProfileDialogData( userId ) ) )
			{
				return;
			}

			await ShowHostedDialog<ProfileDialog, IProfileDialogViewModel, object>();
		}

		public async Task ViewProfile( string screenName )
		{
			if( !DialogStack.Push( new ProfileDialogData( screenName ) ) )
			{
				return;
			}

			await ShowHostedDialog<ProfileDialog, IProfileDialogViewModel, object>();
		}

		public async Task ViewStatus( StatusViewModel status )
		{
			if( status == null )
			{
				return;
			}

			if( !DialogStack.Push( new StatusData( status ) ) )
			{
				return;
			}

			await ShowHostedDialog<TweetDetailsDialog, ITweetDetailsViewModel>();
		}

		private Task<object> ShowHostedDialog<TControl, TViewModel>()
			where TViewModel : class
			where TControl : UserControl, new()
		{
			return ShowHostedDialog<TControl, TViewModel, object>();
		}

		private async Task<TResult> ShowHostedDialog<TControl, TViewModel, TResult>()
			where TViewModel : class
			where TResult : class
			where TControl : UserControl, new()
		{
			var ctrl = new TControl();
			var vm = ctrl.DataContext as TViewModel;

			bool newHost = false;
			DialogWindowHost host = CurrentDialogHost;
			if( host == null )
			{
				host = new DialogWindowHost
				{
					Owner = Window,
					Content = ctrl
				};

				host.Closed += ( s, e ) =>
				{
					DialogStack.Clear();
					CurrentDialogHost = null;
				};

				CurrentDialogHost = host;
				newHost = true;
			}
			else
			{
				host.Content = ctrl;
			}

			var hostVm = host.DataContext as IDialogHostViewModel;
			Debug.Assert( hostVm != null, "hostVm != null" );
			await hostVm.Setup( vm );

			TResult result = null;
			await Dispatcher.RunAsync( () =>
			{
				bool shouldSetupResult = false;

				if( newHost )
				{
					try
					{
						shouldSetupResult = host.ShowDialog() == true;
					}
					catch( InvalidOperationException )
					{
						// Window was closed during setup
					}
				}

				if( shouldSetupResult )
				{
					result = DialogStack.ResultSetup<TViewModel, TResult>( vm );
				}
			} );

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

			if( dlg.ShowDialog() != true )
			{
				return Task.FromResult<TResult>( null );
			}

			Func<TViewModel, TResult> defaultResultSetup = _ => default( TResult );
			var resSetup = resultSetup ?? defaultResultSetup;
			var result = resSetup( vm );

			return Task.FromResult( result );
		}

		private TResult ShowWindowSync<TWindow, TViewModel, TResult>( Func<TViewModel, TResult> resultSetup = null,
			Action<TViewModel> vmSetup = null )
			where TViewModel : class
			where TResult : class
			where TWindow : Window, new()
		{
			var waitHandle = new ManualResetEventSlim( false );

			TResult result = null;

			Dispatcher.CheckBeginInvokeOnUI(
				async () =>
				{
					result = await ShowWindow<TWindow, TViewModel, TResult>( resultSetup, vmSetup );
					waitHandle.Set();
				} );

			waitHandle.Wait();
			return result;
		}

		public void OpenNotificationPopup( NotificationViewModel vm )
		{
			var window = new NotifyPopup()
			{
				DataContext = vm
			};

			window.Top = vm.WindowRect.Top;
			window.Left = vm.WindowRect.Left;

			vm.Reset();
			window.Show();
		}

		[Inject]

		// ReSharper disable once MemberCanBePrivate.Global
		public IDispatcher Dispatcher { get; set; }

		private static MetroWindow Window
			=> Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault( x => x.IsActive );

		private readonly IDialogStack DialogStack;
		private DialogWindowHost CurrentDialogHost;
		private Flyout CurrentlyOpenNotification;
	}
}