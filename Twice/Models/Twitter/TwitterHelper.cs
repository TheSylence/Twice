using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using LinqToTwitter;

namespace Twice.Models.Twitter
{
	internal static class TwitterHelper
	{
		public static int CountCharacters( string text, ITwitterConfiguration twitterConfig )
		{
			text = ReplaceUrls( text, twitterConfig.UrlLength, twitterConfig.UrlLengthHttps );

			try
			{
				var norm = NormalizeText( text );

				var inf = new StringInfo( norm );
				return inf.LengthInTextElements;
			}
			catch( ArgumentException )
			{
				return text.Length;
			}
		}

		public static ulong GetMessageId( this DirectMessage message )
		{
			return message.ID != 0
				? message.ID
				: ulong.Parse( message.IDString );
		}

		public static string GetScreenName( this User user )
		{
			return user.ScreenName ?? user.ScreenNameResponse;
		}

		public static Uri GetUrl( this Status status )
		{
			string userName = status.User.GetScreenName();
			return
				new Uri( string.Format( CultureInfo.InvariantCulture, "https://twitter.com/{0}/status/{1}", userName,
					status.StatusID ) );
		}

		public static ulong GetStatusId( this Status status )
		{
			return status.ID != 0 ? status.ID : status.StatusID;
		}

		public static ulong GetUserId( this User user )
		{
			return user.UserID != 0
				? user.UserID
				: ulong.Parse( user.UserIDResponse );
		}

		public static string NormalizeText( string text )
		{
			try
			{
				return text.Normalize( NormalizationForm.FormC );
			}
			catch( ArgumentException )
			{
				return text;
			}
		}
		
		private static string ReplaceUrls( string text, int httpLength, int httpsLength )
		{
			string httpsString = new string( 'x', httpsLength );

			if( !text.Contains( "." ) && !text.Contains( ":" ) )
			{
				return text;
			}

			MatchCollection matcher = Text.Regex.VALID_URL.Matches( text );
			foreach( Match match in matcher )
			{
				if( !match.Groups[Text.Regex.VALID_URL_GROUP_PROTOCOL].Success )
				{
					// Skip if protocol is not present and 'extractURLWithoutProtocol' is false or
					// URL is preceded by invalid character.
					if(
						Text.Regex.INVALID_URL_WITHOUT_PROTOCOL_MATCH_BEGIN.IsMatch( match.Groups[Text.Regex.VALID_URL_GROUP_BEFORE].Value ) )
					{
						continue;
					}
				}

				string url = match.Groups[Text.Regex.VALID_URL_GROUP_URL].Value;
				
				Match tcoMatcher = Text.Regex.VALID_TCO_URL.Match( url );
				if( tcoMatcher.Success )
				{
					// In the case of t.co URLs, don't allow additional path characters.
					url = tcoMatcher.Value;
				}

				text = text.Replace( url, httpsString );
			}

			return text;
		}
	}
}