using System;
using System.Diagnostics.CodeAnalysis;

namespace Twice
{
	/// <summary>Class containing constants that are used throughout the application.</summary>
	[ExcludeFromCodeCoverage]
	internal static class Constants
	{
		public static class Cache
		{
			internal static TimeSpan UserInfoExpiration = TimeSpan.FromDays( 100 );
		}

		public static class IO
		{
			internal const string ConfigFileName = "config.json";
			internal const string ColumnDefintionFileName = "columns.json";
			internal const string AccountsFileName = "accounts.json";
		}

		public static class Auth
		{
			internal static readonly string ConsumerKey = Obscurity.Deobscure( "==gV1Mlc2V2b5MTNhZHbzdEM4pFdVVke0J3T" );
			internal static readonly string ConsumerSecret = Obscurity.Deobscure( "=g3c1RGboNTRXFTe1IEUiBzTCdjWY1ERxgGNHJFN24UYwJ0YvNDc6N2crNVVkR0U380V" );
		}

		/// <summary>Constants associated with twitter.</summary>
		public static class Twitter
		{
			/// <summary>Prefix for a hashtag.</summary>
			internal const string HashTag = "#";

			/// <summary>Maximum characters allowed in a tweet.</summary>
			internal const int MaxTweetLength = 140;

			/// <summary>Prefix for a user mention.</summary>
			internal const string Mention = "@";
		}
	}
}