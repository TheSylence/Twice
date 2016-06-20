using System;
using System.Threading.Tasks;

namespace Twice.Models.Media
{
	internal interface IMediaExtractor
	{
		bool CanExtract( string originalUrl );

		Task<Uri> GetMediaUrl( string originalUrl );
	}
}