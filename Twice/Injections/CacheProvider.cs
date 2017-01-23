using Ninject.Activation;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Cache;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class CacheProvider : Provider<ICache>
	{
		/// <summary>
		///  Creates an instance within the specified context. 
		/// </summary>
		/// <param name="context"> The context. </param>
		/// <returns> The created instance. </returns>
		protected override ICache CreateInstance( IContext context )
		{
			var sb = new SQLiteConnectionStringBuilder
			{
				DataSource = Constants.IO.CacheFileName,
				JournalMode = SQLiteJournalModeEnum.Wal,
				Enlist = true,
				// default size of a cluster in NTFS file system
				PageSize = 4096,
				// speed up cache operations by not waiting for file system flushs
				SyncMode = SynchronizationModes.Off
			};

			return new SqliteCache( sb.ToString() );
		}
	}
}