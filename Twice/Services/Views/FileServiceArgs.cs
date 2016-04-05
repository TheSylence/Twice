using System.Diagnostics.CodeAnalysis;

namespace Twice.Services.Views
{

	[ExcludeFromCodeCoverage]
	class FileServiceArgs
	{
		public FileServiceArgs( params string[] fileTypes )
		{
			Filter = string.Join( "|", fileTypes );
		}

		public string Filter { get; }
	}
}