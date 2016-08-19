using Anotar.NLog;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Twice.Models.Media
{
	internal class InstragramExtractor : IMediaExtractor
	{
		public bool CanExtract( string originalUrl )
		{
			return Pattern.IsMatch( originalUrl );
		}

		public async Task<Uri> GetMediaUrl( string originalUrl )
		{
			string url = $"https://api.instagram.com/oembed/?url={originalUrl}";

			try
			{
				using( var client = new WebClient() )
				{
					var json = await client.DownloadStringTaskAsync( url );

					var info = JsonConvert.DeserializeObject<InstagramResponse>( json );
					return new Uri( info.thumbnail_url );
				}
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to load image from instagram", ex );
				return null;
			}
		}

		private static readonly Regex Pattern = new Regex( "(?:http(?:s)?:\\/\\/)?(?:www\\.)?instagram\\.com\\/p\\/[\\w-]+(?:\\/)?",
			RegexOptions.Compiled );

		[ExcludeFromCodeCoverage]
		private class InstagramResponse
		{
			// ReSharper disable once InconsistentNaming
			public string thumbnail_url { get; set; }
		}
	}
}