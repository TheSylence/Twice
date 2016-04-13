using System.IO;

namespace Twice.Utilities
{
	internal class FileSystem : IFileSystem
	{
		public bool FileExists( string fileName )
		{
			return File.Exists( fileName );
		}
	}
}