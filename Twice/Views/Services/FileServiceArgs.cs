using System.Diagnostics.CodeAnalysis;

namespace Twice.Views.Services
{
	[ExcludeFromCodeCoverage]
	internal class FileServiceArgs
	{
		public FileServiceArgs( params string[] fileTypes )
		{
			Filter = string.Join( "|", fileTypes );
		}

		public string Filter { get; }
	}
}