using System;
using System.Diagnostics.CodeAnalysis;
using Akavache.Sqlite3;

// Note: This class file is *required* for iOS to work correctly, and is also a good idea for Android
// if you enable "Link All Assemblies".

namespace Twice
{
	[Preserve]
	[ExcludeFromCodeCoverage]
	public static class LinkerPreserve
	{
		static LinkerPreserve()
		{
			throw new Exception( typeof(SQLitePersistentBlobCache).FullName );
		}
	}
}