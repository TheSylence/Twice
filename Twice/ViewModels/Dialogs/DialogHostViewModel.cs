using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Twice.ViewModels.Dialogs.Data;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Dialogs
{
	internal class DialogHostViewModel : IDialogHostViewModel, IContentChanger
	{
		public DialogHostViewModel( IDialogStack stack )
		{
			Stack = stack;
		}

		public event EventHandler<ContentChangeEventArgs> ContentChange;

		public void ChangeContent( object newContent )
		{
			ContentChange?.Invoke( this, new ContentChangeEventArgs( newContent ) );

			// FIXME: This is one hell of an ugly hack...
			CurrentDataContext = ( (UserControl)newContent ).DataContext;
		}

		public async Task Setup<TViewModel>( TViewModel vm ) where TViewModel : class
		{
			// Setup must be called before VM is loaded
			Stack.Setup( vm );

			var loadVm = vm as ILoadCallback;
			if( loadVm != null )
			{
				await loadVm.OnLoad( null );
			}
		}

		private bool CanExecuteBackCommand()
		{
			return Stack.CanGoBack();
		}

		private async void ExecuteBackCommand()
		{
			Stack.Remove();
			Stack.Setup( (IContentChanger)this );

			var loadVm = CurrentDataContext as ILoadCallback;
			if( loadVm != null )
			{
				await loadVm.OnLoad( null );
			}
		}

		public ICommand BackCommand
			=> _BackCommand ?? ( _BackCommand = new RelayCommand( ExecuteBackCommand, CanExecuteBackCommand ) );

		private readonly IDialogStack Stack;
		private RelayCommand _BackCommand;
		private object CurrentDataContext;
	}
}