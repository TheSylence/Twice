using System;
using System.Threading.Tasks;

namespace Twice.Models.Media
{
	internal interface IMediaExtractorRepository
	{
		void AddExtractor( IMediaExtractor extractor );

		Task<Uri> ExtractMedia( string originalUrl );
	}
}