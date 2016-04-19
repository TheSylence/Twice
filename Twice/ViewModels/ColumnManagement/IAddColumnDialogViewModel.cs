using System.Windows.Input;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal interface IAddColumnDialogViewModel : IWizardViewModel
	{
		ICommand AddColumnTypeCommand { get; }
		ICommand SelectAccountCommand { get; }
	}
}