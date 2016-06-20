using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Twice.Models.Media
{
	internal class MediaExtractorRepository : IMediaExtractorRepository
	{
		public void AddExtractor( IMediaExtractor extractor )
		{
			Extractors.Add( extractor );
		}

		public async Task<Uri> ExtractMedia( string originalUrl )
		{
			var extractor = Extractors.FirstOrDefault( e => e.CanExtract( originalUrl ) );
			if( extractor == null )
			{
				return null;
			}

			return await extractor.GetMediaUrl( originalUrl );
		}

		internal static IMediaExtractorRepository Default { get; } = new MediaExtractorRepository();
		private readonly List<IMediaExtractor> Extractors = new List<IMediaExtractor>();
	}
}