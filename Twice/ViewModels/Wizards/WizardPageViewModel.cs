namespace Twice.ViewModels.Wizards
{
	internal abstract class WizardPageViewModel
	{
		public virtual bool CanNavigateForward()
		{
			return true;
		}

		public bool IsLastPage { get; protected set; } = false;

		public void SetNextPage( int key )
		{
			NextPageKey = key;
		}

		public virtual void OnNavigatedFrom( bool backward )
		{
		}

		public virtual void OnNavigatedTo( bool forward )
		{
		}

		public virtual int NextPageKey { get; protected set; }
	}
}