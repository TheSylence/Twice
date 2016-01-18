using Ninject.Modules;
using Twice.Services;
using Twice.Services.ViewServices;

namespace Twice.Injections
{
	internal class ServiceInjectionModule : NinjectModule

	{
		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IServiceRepository>().To<ServiceRepository>().InSingletonScope();

			Bind<ISettingsService>().To<SettingsService>();

			Bind<IFileSelectService>().To<FileSelectService>();
		}
	}
}