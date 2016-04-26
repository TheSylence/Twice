using System.Windows.Input;

namespace Twice.ViewModels.Wizards
{
	internal interface IWizardViewModel : IDialogViewModel
	{
		TValue GetProperty<TValue>( string key );

		void GotoPage( int key );

		void SetProperty( string key, object value );

		WizardPageViewModel CurrentPage { get; set; }
		ICommand FinishCommand { get; }
		ICommand GotoPrevPageCommand { get; }
	}
}