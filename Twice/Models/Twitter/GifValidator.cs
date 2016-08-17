using System.Drawing;
using System.Drawing.Imaging;

namespace Twice.Models.Twitter
{
	internal static class GifValidator
	{
		public enum ValidationResult
		{
			Ok,
			TooWide,
			TooHigh,
			TooManyFrames,
			TooManyPixels
		}

		public static ValidationResult Validate( string fileName )
		{
			var gif = Image.FromFile( fileName );

			if( gif.Size.Width > MaxWidth )
			{
				return ValidationResult.TooWide;
			}
			if( gif.Size.Height > MaxHeight )
			{
				return ValidationResult.TooHigh;
			}

			var dimension = new FrameDimension( gif.FrameDimensionsList[0] );
			int frames = gif.GetFrameCount( dimension );
			if( frames > MaxFrames )
			{
				return ValidationResult.TooManyFrames;
			}

			var pixels = gif.Size.Width * gif.Size.Height * frames;
			if( pixels > MaxPixels )
			{
				return ValidationResult.TooManyPixels;
			}

			return ValidationResult.Ok;
		}

		private const int MaxFrames = 350;
		private const int MaxHeight = 1080;
		private const int MaxPixels = 300000000;
		private const int MaxWidth = 1280;
	}
}