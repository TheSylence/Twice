using System.Windows.Input;
using Twice.ViewModels.Validation;

namespace Twice.ViewModels
{
	internal interface IDialogViewModel : IViewController, IValidationViewModel
	{
		ICommand CancelCommand { get; }
		ICommand OkCommand { get; }
		string Title { get; set; }
	}
}