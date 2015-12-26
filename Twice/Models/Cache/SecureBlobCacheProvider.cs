using Akavache;
using Akavache.Sqlite3;
using Ninject.Activation;

namespace Twice.Models.Cache
{
	internal class SecureBlobCacheProvider : Provider<ISecureBlobCache>
	{
		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// The created instance.
		/// </returns>
		protected override ISecureBlobCache CreateInstance( IContext context )
		{
			if( Cache == null )
			{
				Cache = new SQLiteEncryptedBlobCache( "cache.crypt.db3" );
			}

			return Cache;
		}

		private static ISecureBlobCache Cache;
	}
}