using System;
using System.Threading.Tasks;
using Twice.Models.Twitter;

namespace Twice.Models.Media
{
	internal class GenericMediaExtractor : IMediaExtractor
	{
		public bool CanExtract( string originalUrl )
		{
			var mime = TwitterHelper.GetMimeType( originalUrl );

			return mime.StartsWith( "image/" );
		}

		public Task<Uri> GetMediaUrl( string originalUrl )
		{
			return Task.FromResult( new Uri( originalUrl ) );
		}
	}
}