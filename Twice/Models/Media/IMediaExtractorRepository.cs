using System;

namespace Twice.Models.Media
{
	internal interface IMediaExtractorRepository
	{
		void AddExtractor( IMediaExtractor extractor );

		Uri ExtractMedia( string originalUrl );
	}
}