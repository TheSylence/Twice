using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Twice.ViewModels.Info
{
	internal interface IInfoDialogViewModel : IDialogViewModel
	{
		DateTime BuildDate { get; }
		string Version { get; }
	}

	internal class InfoDialogViewModel : DialogViewModel, IInfoDialogViewModel
	{
		public InfoDialogViewModel()
		{
			var assembly = Assembly.GetExecutingAssembly();

			BuildDate = ExtractBuildDate( assembly.Location );

			//FileVersionInfo info = FileVersionInfo.GetVersionInfo( assembly.Location );
			Version = assembly.GetName().Version.ToString();
		}

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		private static DateTime ExtractBuildDate( string filePath )
		{
			if( DateTime.Now.Year < 2038 )
			{
				try
				{
					const int PeHeaderOffset = 60;
					const int LinkerTimestampOffset = 8;
					byte[] b = new byte[2048];
					using( Stream s = File.Open( filePath, FileMode.Open, FileAccess.Read ) )
					{
						s.Read( b, 0, 2048 );
					}

					int i = BitConverter.ToInt32( b, PeHeaderOffset );

					int SecondsSince1970 = BitConverter.ToInt32( b, i + LinkerTimestampOffset );
					DateTime dt = new DateTime( 1970, 1, 1, 0, 0, 0 );
					dt = dt.AddSeconds( SecondsSince1970 );

					int utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset( dt ).Hours;

					dt = dt.AddHours( utcOffset );
					return dt;
				}
				catch( IOException )
				{
				}
			}

			return File.GetLastWriteTime( filePath );
		}

		public string Version { get; }
		public DateTime BuildDate { get; }
	}
}