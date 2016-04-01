using System.Windows.Input;

namespace Twice.ViewModels.Wizards
{
	internal interface IWizardViewModel : IDialogViewModel
	{
		WizardPageViewModel CurrentPage { get; set; }
		ICommand GotoNextPageCommand { get; }
		ICommand GotoPrevPageCommand { get; }
	}
}