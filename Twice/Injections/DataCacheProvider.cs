using Ninject.Activation;
using Twice.Models.Cache;

namespace Twice.Injections
{
	class DataCacheProvider : Provider<IDataCache>
	{
		/// <summary>Creates an instance within the specified context.</summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		protected override IDataCache CreateInstance( IContext context )
		{
			return new DataCache( Constants.IO.CacheFileName );
		}
	}
}