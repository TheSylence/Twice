using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class FinishPage : WizardPageViewModel
	{
		public FinishPage( WizardViewModel wizard )
			: base( wizard )
		{
			IsLastPage = true;
		}
	}
}