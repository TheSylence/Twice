using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Twitter;

namespace Twice.Tests.Models.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class GifValidatorTests
	{
		[TestMethod, TestCategory( "Models.Twitter" )]
		public void ImageWithTooManyFramesIsRejected()
		{
			// Arrange
			const string fileName = "Data/Gifs/TooManyFrames.gif";

			// Act
			var result = GifValidator.Validate( fileName );

			// Assert
			Assert.AreEqual( GifValidator.ValidationResult.TooManyFrames, result );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void ImageWithTooManyPixelsIsRejected()
		{
			// Arrange
			const string fileName = "Data/Gifs/TooManyPixels.gif";

			// Act
			var result = GifValidator.Validate( fileName );

			// Assert
			Assert.AreEqual( GifValidator.ValidationResult.TooManyPixels, result );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void TooHighImageIsRejected()
		{
			// Arrange
			const string fileName = "Data/Gifs/TooHigh.gif";

			// Act
			var result = GifValidator.Validate( fileName );

			// Assert
			Assert.AreEqual( GifValidator.ValidationResult.TooHigh, result );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void TooWideImageIsRejected()
		{
			// Arrange
			const string fileName = "Data/Gifs/TooWide.gif";

			// Act
			var result = GifValidator.Validate( fileName );

			// Assert
			Assert.AreEqual( GifValidator.ValidationResult.TooWide, result );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void ValidImageIsAccepted()
		{
			// Arrange
			const string fileName = "Data/Gifs/Valid.gif";

			// Act
			var result = GifValidator.Validate( fileName );

			// Assert
			Assert.AreEqual( GifValidator.ValidationResult.Ok, result );
		}
	}
}