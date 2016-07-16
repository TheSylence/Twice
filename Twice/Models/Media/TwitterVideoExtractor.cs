using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twice.Models.Proxy;

namespace Twice.Models.Media
{
	internal class TwitterVideoExtractor : IMediaExtractor
	{
		public bool CanExtract( string originalUrl )
		{
			return Pattern.IsMatch( originalUrl );
		}

		public Task<Uri> GetMediaUrl( string originalUrl )
		{
			bool https = originalUrl.StartsWith( "https:", StringComparison.Ordinal );

			Uri url;
			if( https )
			{
				url = MediaProxyServer.BuildUrl( originalUrl );
			}
			else
			{
				url = new Uri( originalUrl );
			}

			return Task.FromResult( url );
		}

		private static readonly Regex Pattern =
			new Regex( "http[s]?:\\/\\/amp.twimg.com\\/v\\/[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$" );
	}
}