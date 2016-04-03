namespace Twice.Services.Views
{

	class FileServiceArgs
	{
		public FileServiceArgs( params string[] fileTypes )
		{
			Filter = string.Join( "|", fileTypes );
		}

		public string Filter { get; }
	}
}