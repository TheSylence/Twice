using System.IO;

namespace Twice.Utilities
{
	static class ResourceHelper
	{
		public static string GetDefaultNotificationSound ()
		{
			var defaultFileName = Path.GetFullPath( Constants.IO.DefaultNotificationSoundFile );

			if( !File.Exists( defaultFileName ) )
			{
				using( var resStream = Resourcer.Resource.AsStream( "Twice.Resources.Data.HumanBird.wav" ) )
				using( var fileStream = File.OpenWrite( defaultFileName ) )
				{
					resStream.CopyTo( fileStream );
				}
			}

			return defaultFileName;
		}
	}
}
