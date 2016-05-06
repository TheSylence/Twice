using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Resourcer;

namespace Twice.ViewModels.Info
{
	internal class InfoDialogViewModel : DialogViewModel, IInfoDialogViewModel
	{
		public InfoDialogViewModel()
		{
			var assembly = Assembly.GetExecutingAssembly();

			BuildDate = ExtractBuildDate( assembly.Location );

			//FileVersionInfo info = FileVersionInfo.GetVersionInfo( assembly.Location );
			Version = assembly.GetName().Version.ToString();

			Licenses = ReadLicenses( assembly ).OrderBy( l => l.Name ).ToList();

			var changelogJson = Resource.AsString( "Twice.Resources.Texts.Changelog.json" );
			Changelogs = JsonConvert.DeserializeObject<List<ChangelogItem>>( changelogJson );
		}

		[ExcludeFromCodeCoverage]
		private static DateTime ExtractBuildDate( string filePath )
		{
			if( DateTime.Now.Year < 2038 )
			{
				try
				{
					const int peHeaderOffset = 60;
					const int linkerTimestampOffset = 8;
					byte[] b = new byte[2048];
					using( Stream s = File.Open( filePath, FileMode.Open, FileAccess.Read ) )
					{
						s.Read( b, 0, 2048 );
					}

					int i = BitConverter.ToInt32( b, peHeaderOffset );

					int secondsSince1970 = BitConverter.ToInt32( b, i + linkerTimestampOffset );
					DateTime dt = new DateTime( 1970, 1, 1, 0, 0, 0 );
					dt = dt.AddSeconds( secondsSince1970 );

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
		public ICollection<ChangelogItem> Changelogs { get; }
		public ICollection<LicenseItem> Licenses { get; }
		public string Version { get; }
	}
}