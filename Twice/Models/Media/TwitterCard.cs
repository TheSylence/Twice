using System.Collections.Generic;

namespace Twice.Models.Media
{
	internal class TwitterCard
	{
		public TwitterCard( Dictionary<string, string> meta )
		{
			string temp;

			if( meta.TryGetValue( "twitter:card", out temp ) )
			{
				switch( temp )
				{
				case "summary":
					Type = CardType.Summary;
					break;

				case "summary_large_image":
					Type = CardType.SummaryLargeImage;
					break;

				default:
					Type = CardType.Unknown;
					break;
				}
			}

			if( meta.TryGetValue( "twitter:site", out temp ) )
			{
				Site = temp;
			}

			if( meta.TryGetValue( "twitter:title", out temp ) )
			{
				Title = temp;
			}

			if( meta.TryGetValue( "twitter:description", out temp ) )
			{
				Description = temp;
			}

			if( meta.TryGetValue( "twitter:image", out temp ) )
			{
				Image = temp;
			}
			if( meta.TryGetValue( "twitter:image:alt", out temp ) )
			{
				ImageAlt = temp;
			}
		}

		public string Description { get; }
		public string Image { get; }
		public string ImageAlt { get; }

		public bool IsValid => !string.IsNullOrWhiteSpace( Title )
		                       && !string.IsNullOrWhiteSpace( Description )
		                       && !string.IsNullOrWhiteSpace( Site )
		                       && Type != CardType.Unknown;

		public string Site { get; }
		public string Title { get; }
		public CardType Type { get; }
	}
}