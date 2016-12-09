using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.ViewModels.Dialogs.Data;

namespace Twice.ViewModels.Dialogs
{
	interface IDialogHostViewModel
	{
		ICommand BackCommand { get; }
	}

	class DialogHostViewModel : IDialogHostViewModel
	{
		public DialogHostViewModel( IDialogStack stack )
		{
			Stack = stack;
		}

		private readonly IDialogStack Stack;

		public ICommand BackCommand
			=> _BackCommand ?? ( _BackCommand = new RelayCommand( ExecuteBackCommand, CanExecuteBackCommand ) );

		private void ExecuteBackCommand()
		{
			throw new System.NotImplementedException();
		}

		private bool CanExecuteBackCommand()
		{
			return Stack.CanGoBack();
		}

		private RelayCommand _BackCommand;
	}


}