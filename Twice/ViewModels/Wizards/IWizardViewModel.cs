using System.Windows.Input;

namespace Twice.ViewModels.Wizards
{
	internal interface IWizardViewModel : IDialogViewModel
	{
		WizardPageViewModel CurrentPage { get; set; }
		ICommand FinishCommand { get; }
		ICommand GotoNextPageCommand { get; }
		ICommand GotoPrevPageCommand { get; }
	}
}