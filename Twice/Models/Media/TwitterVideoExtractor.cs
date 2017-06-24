using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Anotar.NLog;

namespace Twice.Models.Media
{
	internal class TwitterVideoExtractor : IMediaExtractor
	{
		public bool CanExtract( string originalUrl )
		{
			return Pattern.IsMatch( originalUrl );
		}

		public async Task<Uri> GetMediaUrl( string originalUrl )
		{
			try
			{
				using( var client = new WebClient() )
				{
					var twitterVideoContent = await client.DownloadStringTaskAsync( originalUrl );
					var iframeUrl = "";

					var iframeContent = await client.DownloadStringTaskAsync( iframeUrl );
					return null;
				}
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to load video from twitter", ex );
				return null;
			}
		}

		private static readonly Regex Pattern =
			new Regex( "http[s]?:\\/\\/amp.twimg.com\\/v\\/[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$" );
	}
}