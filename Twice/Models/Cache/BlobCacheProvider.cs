using Akavache;
using Akavache.Sqlite3;
using Ninject.Activation;

namespace Twice.Models.Cache
{
	internal class BlobCacheProvider : Provider<IBlobCache>
	{
		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// The created instance.
		/// </returns>
		protected override IBlobCache CreateInstance( IContext context )
		{
			if( Cache == null )
			{
				Cache = new SQLitePersistentBlobCache( "cache.db3" );
			}

			return Cache;
		}

		private static IBlobCache Cache;
	}
}