using System;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Twice.Models.Media
{
	internal class InstragramExtractor : IMediaExtractor
	{
		public bool CanExtract( string originalUrl )
		{
			return Pattern.IsMatch( originalUrl );
		}

		public Uri GetMediaUrl( string originalUrl )
		{
			string url = $"https://api.instagram.com/oembed/?url={originalUrl}";

			using( var web = new WebClient() )
			{
				var json = web.DownloadString( url );

				var info = JsonConvert.DeserializeObject<InstagramResponse>( json );
				return new Uri( info.thumbnail_url );
			}
		}

		private class InstagramResponse
		{
			public string thumbnail_url { get; set; }
		}

		private static readonly Regex Pattern = new Regex( "(http(s)?:\\/\\/)?instagram.com\\/p\\/[\\w-]+\\/",
			RegexOptions.Compiled );
	}
}