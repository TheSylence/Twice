using Akavache;
using Ninject.Modules;
using Twice.Models.Cache;
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
			Bind<IBlobCache>().ToProvider<BlobCacheProvider>();
			Bind<ISecureBlobCache>().ToProvider<SecureBlobCacheProvider>();

			Bind<ITwitterContextList>().To<TwitterContextList>().InSingletonScope();
		}
	}
}