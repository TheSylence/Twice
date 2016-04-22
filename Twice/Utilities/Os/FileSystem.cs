using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Twice.Utilities.Os
{
	[ExcludeFromCodeCoverage]
	internal class FileSystem : IFileSystem
	{
		public bool FileExists( string fileName )
		{
			return File.Exists( fileName );
		}
	}
}