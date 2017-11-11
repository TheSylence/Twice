using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class FinishPage : WizardPageViewModel
	{
		public FinishPage( IWizardViewModel wizard )
			: base( wizard )
		{
			IsLastPage = true;
		}

		public override void OnNavigatedTo( bool forward )
		{
			base.OnNavigatedTo( forward );

			SourceAccount = string.Join( ", ", Wizard.GetProperty<string[]>( AddColumnDialogViewModel.SourceAccountNamesKey ) );
		}

		public string ColumnTypeDescription { get; set; }

		public string SourceAccount { get; set; }
	}
}