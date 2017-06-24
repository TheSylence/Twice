using System.IO;
using Resourcer;

namespace Twice.Utilities
{
	internal static class ResourceHelper
	{
		public static string GetDefaultNotificationSound()
		{
			var defaultFileName = Path.GetFullPath( Constants.IO.DefaultNotificationSoundFile );

			if( File.Exists( defaultFileName ) )
			{
				return defaultFileName;
			}

			using( var resStream = Resource.AsStream( "Twice.Resources.Data.HumanBird.wav" ) )
			using( var fileStream = File.OpenWrite( defaultFileName ) )
			{
				resStream.CopyTo( fileStream );
			}

			return defaultFileName;
		}
	}
}