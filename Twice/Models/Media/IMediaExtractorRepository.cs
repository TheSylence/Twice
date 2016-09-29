using System;
using System.Threading.Tasks;

namespace Twice.Models.Media
{
	internal interface IMediaExtractorRepository
	{
		void AddExtractor( IMediaExtractor extractor );

		bool CanExtract( string url );

		Task<Uri> ExtractMedia( string originalUrl );
	}
}