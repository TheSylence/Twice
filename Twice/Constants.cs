using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Twice
{
	/// <summary>
	///     Class containing constants that are used throughout the application.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal static class Constants
	{
		public static class Auth
		{
			internal static readonly string ConsumerKey = Obscurity.Deobscure( "==gV1Mlc2V2b5MTNhZHbzdEM4pFdVVke0J3T" );

			internal static readonly string ConsumerSecret =
				Obscurity.Deobscure( "=g3c1RGboNTRXFTe1IEUiBzTCdjWY1ERxgGNHJFN24UYwJ0YvNDc6N2crNVVkR0U380V" );
		}

		public static class Cache
		{
			internal static TimeSpan UserInfoExpiration = TimeSpan.FromDays( 100 );
			internal static TimeSpan HashtagExpiration = TimeSpan.FromDays( 30 );
		}

		public static class IO
		{
			private static string P( string file )
			{
#if DEBUG
				return file;
#else
				return Path.Combine( AppDataFolder, file );
#endif
			}

			internal static readonly string CacheFileName = P( "cache.db3" );
			internal static readonly string SecureCacheFileName = P( "cache.crypt.db3" );
			internal static readonly string AccountsFileName = P( "accounts.json" );
			internal static readonly string ColumnDefintionFileName = P( "columns.json" );
			internal static readonly string ConfigFileName = P( "config.json" );

			internal static string AppDataFolder
			{
				get
				{
					var localAppData = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );

					var path = Path.Combine( localAppData, "Twice", "data" );
					if( !Directory.Exists( path ) )
					{
						Directory.CreateDirectory( path );
					}
					return path;
				}
			}
		}

		public static class Updates
		{
			internal const string ReleaseChannelUrl = "http://software.btbsoft.org/twice";
			internal const string BetaChannelUrl = "http://software.btbsoft.org/twice/beta";
		}

		/// <summary>
		///     Constants associated with twitter.
		/// </summary>
		public static class Twitter
		{
			/// <summary>
			///     Prefix for a hashtag.
			/// </summary>
			internal const string HashTag = "#";

			/// <summary>
			///     Maximum characters allowed in a tweet.
			/// </summary>
			internal const int MaxTweetLength = 140;

			/// <summary>
			///     Prefix for a user mention.
			/// </summary>
			internal const string Mention = "@";
		}
	}
}