using Akavache;
using Ninject.Modules;
using Twice.Models.Cache;
using Twice.Models.Configuration;
using Twice.Models.Contexts;

namespace Twice.Injections
{
	internal class ModelInjectionModule : NinjectModule
	{
		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IDataCache>().To<DataCache>().InSingletonScope();
			Bind<IBlobCache>().ToProvider<BlobCacheProvider>().InSingletonScope();
			Bind<ISecureBlobCache>().ToProvider<SecureBlobCacheProvider>().InSingletonScope();

			Bind<ITwitterContextList>().To<TwitterContextList>().InSingletonScope();
			Bind<IConfig>().ToProvider<ConfigurationProvider>().InSingletonScope();
		}
	}
}