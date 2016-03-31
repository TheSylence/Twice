using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Twice.ViewModels.Info
{
	internal interface IInfoDialogViewModel : IDialogViewModel
	{
		DateTime BuildDate { get; }
		ICollection<LicenseItem> Licenses { get; }
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

			Licenses = ReadLicenses( assembly ).OrderBy( l => l.Name ).ToList();
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

		private IEnumerable<LicenseItem> ReadLicenses( Assembly assembly )
		{
			const string lookupCrit = ".Resources.Licenses.";
			foreach( var res in assembly.GetManifestResourceNames().Where( n => n.Contains( lookupCrit ) ) )
			{
				using( var stream = assembly.GetManifestResourceStream( res ) )
				{
					Debug.Assert( stream != null, "stream != null" );
					using( var reader = new StreamReader( stream ) )
					{
						int resIdx = res.IndexOf( lookupCrit, StringComparison.Ordinal );
						string name = res.Substring( resIdx + lookupCrit.Length );
						name = name.Substring( 0, name.Length - 4 );

						yield return new LicenseItem( name, reader.ReadToEnd() );
					}
				}
			}
		}

		public DateTime BuildDate { get; }
		public ICollection<LicenseItem> Licenses { get; }
		public string Version { get; }
	}

	internal class LicenseItem
	{
		public LicenseItem( string name, string content )
		{
			Name = name;
			Content = content;
		}

		public string Content { get; }
		public string Name { get; }
	}
}