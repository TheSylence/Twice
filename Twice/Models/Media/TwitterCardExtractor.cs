using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Twice.Models.Media
{
	internal class TwitterCardExtractor : ITwitterCardExtractor
	{
		public async Task<TwitterCard> ExtractCard( Uri url )
		{
			using( var client = new HttpClient() )
			{
				var response = await client.GetAsync( url );
				var str = await response.Content.ReadAsStringAsync();

				return ExtractCard( str );
			}
		}

		private TwitterCard ExtractCard( string http )
		{
			int headEnd = http.IndexOf( "</head>", StringComparison.OrdinalIgnoreCase );
			if( headEnd == -1 )
			{
				return null;
			}

			string html = http.Substring( 0, headEnd ) + "</html>";
			html = WebUtility.HtmlDecode( html );

			var doc = new HtmlDocument();
			doc.LoadHtml( html );

			var head = doc.DocumentNode.SelectSingleNode( "html/head" );
			var metaNodes = head.SelectNodes( "meta" );
			Dictionary<string, string> twitterMetaInfo = new Dictionary<string, string>();

			foreach( var meta in metaNodes )
			{
				var name = meta.Attributes["name"]?.Value;
				var value = meta.Attributes["content"]?.Value;

				if( name?.StartsWith( "twitter:", StringComparison.OrdinalIgnoreCase ) == true && value != null )
				{
					twitterMetaInfo.Add( name, value );
				}
			}

			return new TwitterCard( twitterMetaInfo );
		}

		internal static ITwitterCardExtractor Default { get; } = new TwitterCardExtractor();
	}
}