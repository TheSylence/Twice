using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Models.Media;

namespace Twice.Tests.Models.Media
{
	[TestClass, ExcludeFromCodeCoverage]
	public class TwitterCardExtractorTests
	{
		[TestMethod, TestCategory( "Models.Media" )]
		public void DuplicateMetaDataDoesNotCrash()
		{
			// Arrange
			var html = "<html><head><meta name=\"twitter:test\" value=\"test\" /></head><body>Hello</body></html>";
			var extractor = new TwitterCardExtractor();

			// Act
			var card = extractor.ExtractCard( html );

			// Assert
			Assert.IsNotNull( card );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void MissingHeadEndDoesNotCrash()
		{
			// Arrange
			var html = "<html><head><body>Hello</body></html>";
			var extractor = new TwitterCardExtractor();

			// Act
			var card = extractor.ExtractCard( html );

			// Assert
			Assert.IsNull( card );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void MissingHeadStartDoesNotCrash()
		{
			// Arrange
			var html = "<html></head><body>Hello</body></html>";
			var extractor = new TwitterCardExtractor();

			// Act
			var card = extractor.ExtractCard( html );

			// Assert
			Assert.IsNull( card );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void MissingHeadTagDoesNotCrash()
		{
			// Arrange
			var html = "<html><body>Hello</body></html>";
			var extractor = new TwitterCardExtractor();

			// Act
			var card = extractor.ExtractCard( html );

			// Assert
			Assert.IsNull( card );
		}
	}
}