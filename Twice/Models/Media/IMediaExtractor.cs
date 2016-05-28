using System;

namespace Twice.Models.Media
{
	internal interface IMediaExtractor
	{
		bool CanExtract( string originalUrl );

		Uri GetMediaUrl( string originalUrl );
	}
}