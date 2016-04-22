using GalaSoft.MvvmLight;

namespace Twice.ViewModels.Wizards
{
	internal abstract class WizardPageViewModel : ObservableObject
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

		public void SetNextPage( int key )
		{
			NextPageKey = key;
		}

		public bool IsLastPage { get; protected set; } = false;
		public virtual int NextPageKey { get; protected set; }
	}
}