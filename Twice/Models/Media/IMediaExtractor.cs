using System;
using System.Text;
using System.Threading.Tasks;

namespace Twice.Models.Media
{
	interface IMediaExtractor
	{
		Uri GetMediaUrl( string originalUrl );
		bool CanExtract( string originalUrl );
	}
}
