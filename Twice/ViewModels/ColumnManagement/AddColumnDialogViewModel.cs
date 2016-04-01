using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class AddColumnDialogViewModel : WizardViewModel, IAddColumnDialogViewModel
	{
		public AddColumnDialogViewModel()
		{
			Pages.Add( 0, new ColumnTypeSelctorPage() );

			CurrentPage = Pages[0];
		}
	}
}