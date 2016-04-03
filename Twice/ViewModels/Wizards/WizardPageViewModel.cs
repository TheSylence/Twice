namespace Twice.ViewModels.Wizards
{
	internal abstract class WizardPageViewModel
	{
		public virtual bool CanNavigateForward()
		{
			return true;
		}

		public virtual void OnNavigatedFrom( bool backward )
		{
		}

		public virtual void OnNavigatedTo( bool forward )
		{
		}

		public abstract int NextPageKey { get; }
	}
}