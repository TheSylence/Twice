using Ninject.Modules;
using Twice.Services;
using Twice.Services.Views;

namespace Twice.Injections
{
	internal class ServiceInjectionModule : NinjectModule

	{
		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IViewServiceRepository>().To<ViewServiceRepository>().InSingletonScope();
		}
	}
}