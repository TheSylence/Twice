using System.Diagnostics.CodeAnalysis;

namespace Twice
{
	/// <summary>Class containing constants that are used throughout the application.</summary>
	[ExcludeFromCodeCoverage]
	internal static class Constants
	{
		public static class IO
		{
			internal const string ConfigFileName = "config.json";
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