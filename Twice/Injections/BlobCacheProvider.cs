using Akavache;
using Akavache.Sqlite3;
using Ninject.Activation;

namespace Twice.Injections
{
	internal class BlobCacheProvider : Provider<IBlobCache>
	{
		/// <summary>
		///     Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		protected override IBlobCache CreateInstance( IContext context )
		{
			return Cache ?? ( Cache = new SQLitePersistentBlobCache( Constants.IO.CacheFileName ) );
		}

		private static IBlobCache Cache;
	}
}