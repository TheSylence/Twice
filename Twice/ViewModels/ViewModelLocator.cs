using Ninject;
using System.Diagnostics.CodeAnalysis;
using Twice.ViewModels.Main;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels
{
	[ExcludeFromCodeCoverage]
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