using Newtonsoft.Json;
using System;
using System.Net;
using System.Text.RegularExpressions;

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

		private static readonly Regex Pattern = new Regex( "(http(s)?:\\/\\/)?(www\\.)?instagram\\.com\\/p\\/[\\w-]+(\\/)?",
			RegexOptions.Compiled );

		private class InstagramResponse
		{
			public string thumbnail_url { get; set; }
		}
	}
}