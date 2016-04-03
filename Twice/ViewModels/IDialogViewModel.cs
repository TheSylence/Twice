using System.Windows.Input;

namespace Twice.ViewModels
{
	internal interface IDialogViewModel : IViewController
	{
		ICommand CancelCommand { get; }
		ICommand OkCommand { get; }
		string Title { get; set; }
	}
}