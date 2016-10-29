using Anotar.NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Twice.Utilities.Os;

namespace Twice.Utilities
{
	internal static class VersionReporter
	{
		public static async Task Report()
		{
			using( var client = new WebClient() )
			{
				var query = new VersionInfo().ToQuery();
				string url = $"{Constants.Web.VersionStatsUrl}?{query}";

				try
				{
					await client.DownloadDataTaskAsync( url );
				}
				catch( Exception ex )
				{
					LogTo.WarnException( "Failed to report version info", ex );
				}
			}
		}

		private class VersionInfo
		{
			public VersionInfo()
			{
				WindowsVersion = OsVersionInfo.Version.ToString();
				AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				BitsWindows64 = OsVersionInfo.OsBits == OsVersionInfo.SoftwareArchitecture.Bit64;
				BitsCpu64 = OsVersionInfo.ProcessorBits == OsVersionInfo.ProcessorArchitecture.Bit64;
				BitsApp64 = OsVersionInfo.ProgramBits == OsVersionInfo.SoftwareArchitecture.Bit64;
				WindowsName = $"{OsVersionInfo.Name} {OsVersionInfo.Edition}";
			}

			public string ToQuery()
			{
				var dict = new Dictionary<string, string>
				{
					{"app", AppVersion},
					{"osname", WindowsName},
					{"os", WindowsVersion},
					{"app64", BitsApp64 ? "1" : "0"},
					{"cpu64", BitsCpu64 ? "1" : "0"},
					{"win64", BitsWindows64 ? "1" : "0"}
				};

				return string.Join( "&", dict.Select( kvp => $"{kvp.Key}={WebUtility.UrlEncode( kvp.Value )}" ) );
			}

			private string AppVersion { get; }
			private bool BitsApp64 { get; }
			private bool BitsCpu64 { get; }
			private bool BitsWindows64 { get; }
			private string WindowsName { get; }
			private string WindowsVersion { get; }
		}
	}
}