using Ninject;
using Twice.ViewModels.Main;

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
}