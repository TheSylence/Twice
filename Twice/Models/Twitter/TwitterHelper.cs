using LinqToTwitter;
using System;
using System.Globalization;

namespace Twice.Models.Twitter
{
	internal static class TwitterHelper
	{
		public static string GetScreenName( this User user )
		{
			return user.ScreenName ?? user.ScreenNameResponse;
		}

		public static Uri GetUrl( this Status status )
		{
			string userName = status.User.GetScreenName();
			return new Uri( string.Format( CultureInfo.InvariantCulture, "https://twitter.com/{0}/status/{1}", userName, status.StatusID ) );
		}
	}
}