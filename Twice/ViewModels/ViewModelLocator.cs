using Ninject;
using Twice.ViewModels.Accounts;
using Twice.ViewModels.Main;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Settings;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels
{
	internal class DialogViewModelLocator
	{
		public DialogViewModelLocator()
		{
			Kernel = App.Kernel;
		}

		public IAccountsDialogViewModel Accounts => Kernel.Get<IAccountsDialogViewModel>();

		public IProfileDialogViewModel Profile => Kernel.Get<IProfileDialogViewModel>();
		public ISettingsDialogViewModel Settings => Kernel.Get<ISettingsDialogViewModel>();
		private readonly IKernel Kernel;
	}

	internal class ViewModelLocator
	{
		public ViewModelLocator()
		{
			Kernel = App.Kernel;
		}

		public IComposeTweetViewModel ComposeTweet => Kernel.Get<IComposeTweetViewModel>();
		public IMainViewModel Main => Kernel.Get<IMainViewModel>();

		private readonly IKernel Kernel;
	}
}