using Ninject;
using Twice.ViewModels.Main;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Settings;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels
{
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

	internal class DialogViewModelLocator
	{
		public DialogViewModelLocator()
		{
			Kernel = App.Kernel;
		}

		public ISettingsDialogViewModel Settings => Kernel.Get<ISettingsDialogViewModel>();
		public IProfileDialogViewModel Profile => Kernel.Get<IProfileDialogViewModel>();

		private readonly IKernel Kernel;
	}
}