using System;
using System.Collections.Generic;
using System.Linq;

namespace Twice.Models.Media
{
	static class MediaExtractorRepository
	{
		public static Uri ExtractMedia( string originalUrl )
		{
			var extractor = Extractors.FirstOrDefault( e => e.CanExtract( originalUrl ) );

			return extractor?.GetMediaUrl( originalUrl );
		}

		public static void AddExtractor( IMediaExtractor extractor )
		{
			Extractors.Add( extractor );
		}

		private static readonly List<IMediaExtractor> Extractors = new List<IMediaExtractor>();
	}
}