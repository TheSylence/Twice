using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Anotar.NLog;
using LinqToTwitter;

namespace Twice.Models.Twitter
{
	internal static class TwitterHelper
	{
		/// <summary>
		///     Applies Unicode Normalization to a text and then counts characters.
		/// </summary>
		/// <remarks>
		///     See https://dev.twitter.com/basics/counting-characters for details on how twitter counts characters.
		/// </remarks>
		/// <param name="text"></param>
		/// <param name="twitterConfig"></param>
		/// <returns></returns>
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

		public static ulong ExtractTweetId( string url )
		{
			var match = TweetUrlPattern.Match( url );
			return match.Success != true
				? 0
				: ulong.Parse( match.Groups[2].Value );
		}

		public static ulong GetMessageId( this DirectMessage message )
		{
			return message.ID != 0
				? message.ID
				: ulong.Parse( message.IDString );
		}

		public static string GetMimeType( string fileName )
		{
			if( fileName.IndexOfAny( Path.GetInvalidPathChars() ) == -1 )
			{
				var ext = Path.GetExtension( fileName );

				var lookup = new Dictionary<string, string>
				{
					[".png"] = "image/png",
					[".gif"] = "image/gif",
					[".bmp"] = "image/bmp",
					[".jpg"] = "image/jpg",
					[".jpeg"] = "image/jpg"
				};

				if( !string.IsNullOrEmpty( ext ) && lookup.ContainsKey( ext ) )
				{
					return lookup[ext];
				}
			}

			return "application/octet-stream";
		}

		public static string GetScreenName( this User user )
		{
			return user.ScreenName ?? user.ScreenNameResponse;
		}

		public static ulong GetStatusId( this Status status )
		{
			if( status == null )
			{
				return 0;
			}

			return status.ID != 0
				? status.ID
				: status.StatusID;
		}

		public static Uri GetUrl( this Status status )
		{
			string userName = status.User.GetScreenName();
			return
				new Uri( string.Format( CultureInfo.InvariantCulture, "https://twitter.com/{0}/status/{1}", userName,
					status.StatusID ) );
		}

		public static ulong GetUserId( this User user )
		{
			try
			{
				return user.UserID != 0
					? user.UserID
					: ulong.Parse( user.UserIDResponse );
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "", ex );
				return 0;
			}
		}

		public static ulong GetUserId( this Status status )
		{
			var id = status.UserID;
			if( id != 0 )
			{
				return id;
			}

			if( ulong.TryParse( status.User.UserIDResponse, out id ) && id != 0 )
			{
				return id;
			}

			id = status.User.GetUserId();
			return id;
		}

		public static Uri GetUserUrl( this User user )
		{
			string userName = user.GetScreenName();
			return new Uri( string.Format( CultureInfo.InvariantCulture, "https://twitter.com/{0}", userName ) );
		}

		public static bool IsExtendedTweetUrl( string url )
		{
			return url.ToLowerInvariant().Contains( "/i/web/status/" );
		}

		public static bool IsSupportedImage( string fileName )
		{
			var mime = GetMimeType( fileName );

			return mime.StartsWith( "image/", StringComparison.Ordinal );
		}

		public static bool IsTweetUrl( Uri uri )
		{
			return IsTweetUrl( uri.AbsoluteUri );
		}

		public static bool IsTweetUrl( string url )
		{
			return TweetUrlPattern.IsMatch( url );
		}

		public static string NormalizeText( string text )
		{
			if( string.IsNullOrWhiteSpace( text ) )
			{
				return text;
			}

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
			if( string.IsNullOrEmpty( text ) )
			{
				return string.Empty;
			}

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

		private static readonly Regex TweetUrlPattern = new Regex( "(https:\\/\\/)?twitter.com\\/\\w+\\/status\\/(\\d+)",
			RegexOptions.Compiled );
	}
}