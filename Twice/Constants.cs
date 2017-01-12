using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Twice
{
	/// <summary>
	///  Class containing constants that are used throughout the application. 
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal static partial class Constants
	{
		public static class Gui
		{
			/// <summary>
			///  Number of user pictures for retweets to display in the detail dialog 
			/// </summary>
			internal const int MaxRetweets = 10;
		}

		public static class Web
		{
			internal const string CrashReportUrl = "http://software.btbsoft.org/twice/crash/report.php";
			internal const string VersionStatsUrl = "http://software.btbsoft.org/twice/stats/report.php";
		}

		public static class Cache
		{
			internal static TimeSpan UserInfoExpiration = TimeSpan.FromDays( 2 );
			internal static TimeSpan HashtagExpiration = TimeSpan.FromDays( 30 );
			internal static TimeSpan TwitterConfigExpiration = TimeSpan.FromDays( 1 );
			internal static TimeSpan StatusExpiration = TimeSpan.FromDays( 3 );
			internal static TimeSpan MessageExpiration = TimeSpan.FromDays( 7 );
		}

		public static class IO
		{
			private static string P( string file )
			{
#if DEBUG
				return file;
#else
				return Path.Combine( RoamingAppDataFolder, file );
#endif
			}

			internal static readonly string CacheFileName = P( "cache.db3" );
			internal static readonly string AccountsFileName = P( "accounts.json" );
			internal static readonly string ColumnDefintionFileName = P( "columns.json" );
			internal static readonly string ConfigFileName = P( "config.json" );
			internal static readonly string WindowSettingsFileName = P( "window.json" );
			internal static readonly string DefaultNotificationSoundFile = P( "HumanBird.wav" );
			internal static readonly string SchedulerFileName = P( "scheduler.json" );

			internal static string RoamingAppDataFolder
			{
				get
				{
					var roamingAppData = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
					var path = Path.Combine( roamingAppData, "Twice" );
					if( !Directory.Exists( path ) )
					{
						Directory.CreateDirectory( path );
					}
					return path;
				}
			}

			internal static string AppDataFolder
			{
				get
				{
					var localAppData = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );

					var path = Path.Combine( localAppData, "Twice", "data" );
					return path;
				}
			}
		}

		/// <summary>
		///  Constants associated with twitter. 
		/// </summary>
		public static class Twitter
		{
			/// <summary>
			///  Prefix for a hashtag. 
			/// </summary>
			internal const string HashTag = "#";

			/// <summary>
			///  Maximum characters allowed in a tweet. 
			/// </summary>
			internal const int MaxTweetLength = 140;

			/// <summary>
			///  Prefix for a user mention. 
			/// </summary>
			internal const string Mention = "@";

			public static class ErrorCodes
			{
				public const int CouldNotAuthenticateYou = 32;
				public const int PageDoesNotExist = 34;
				public const int AccountSuspended = 64;
				public const int RateLimitExceeded = 88;
				public const int InvalidOrExpiredToken = 89;
				public const int OverCapacity = 130;
				public const int InternalError = 131;
				public const int CouldNotAuthenticateYouTimestamp = 135;
				public const int UnableToFollowMorePeople = 161;
				public const int NotAuthorizedToSeePost = 179;
				public const int OverDailyStatusUpdateLimit = 185;
				public const int StatusDuplicate = 187;
			}
		}

		// Debug constant to keep #if out of other files because of CodeMaid problems with
		// preprocessor directives
		internal static bool Debug
		{
			get
			{
#if DEBUG
				return true;
#else
				return false;
#endif
			}
		}

		internal const string ApplicationId = "com.squirrel.twice.Twice";
	}
}