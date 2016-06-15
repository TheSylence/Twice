using System;
using System.Linq;
using System.Web;

namespace Twice.Models.Media
{
	internal class YoutubeExtractor : IMediaExtractor
	{
		private static Uri BuildThumbnailUri( string videoId )
		{
			return new Uri( $"http://img.youtube.com/vi/{videoId}/0.jpg" );
		}

		public bool CanExtract( string originalUrl )
		{
			var uri = new Uri( originalUrl );
			if( uri.Host == "youtu.be" || uri.Host == "www.youtu.be" )
			{
				return true;
			}

			if( uri.Host == "youtube.com" || uri.Host == "www.youtube.com" )
			{
				if( !originalUrl.Contains( "?" ) )
				{
					return false;
				}

				var queryString = originalUrl.Substring( originalUrl.IndexOf( '?' ) ).Split( '#' )[0];
				var args = HttpUtility.ParseQueryString( queryString );

				if( !args.AllKeys.Contains( "v" ) )
				{
					return false;
				}

				var v = args.GetValues( "v" );
				return v.Any( x => !string.IsNullOrWhiteSpace( x ) );
			}

			return false;
		}

		public Uri GetMediaUrl( string originalUrl )
		{
			string videoId = null;

			var uri = new Uri( originalUrl );
			if( uri.Host == "youtu.be" || uri.Host == "www.youtu.be" )
			{
				videoId = uri.Segments.Last();
			}

			if( uri.Host == "youtube.com" || uri.Host == "www.youtube.com" )
			{
				var queryString = originalUrl.Substring( originalUrl.IndexOf( '?' ) ).Split( '#' )[0];
				var args = HttpUtility.ParseQueryString( queryString );

				var v = args.GetValues( "v" );
				videoId = v.FirstOrDefault( x => !string.IsNullOrWhiteSpace( x ) );
			}

			return BuildThumbnailUri( videoId );
		}
	}
}