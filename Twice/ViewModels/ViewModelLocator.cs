using Ninject;
using Twice.ViewModels.Main;
using Twice.ViewModels.Settings;

namespace Twice.ViewModels
{
	internal class ViewModelLocator
	{
		public ViewModelLocator()
		{
			Kernel = App.Kernel;
		}

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

		private readonly IKernel Kernel;
	}
}