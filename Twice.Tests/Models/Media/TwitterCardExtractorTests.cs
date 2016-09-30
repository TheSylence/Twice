using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
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
		public async Task ExtendedTwitterStatusUrlsAreNotExtracted()
		{
			// Arrange
			var urls = new[]
			{
				"https://twitter.com/i/web/status/123",
				"http://twitter.com/i/web/status/123",
				"https://www.twitter.com/i/web/status/123",
				"https://tWitter.com/i/web/status/123",
				"http://TWITTER.Com/i/web/status/123"
			};

			var extractor = new TwitterCardExtractor();

			// Act
			var cardTasks = urls.Select( u => extractor.ExtractCard( new System.Uri( u ) ) ).ToArray();

			await Task.WhenAll( cardTasks );

			// Assert
			Assert.IsTrue( cardTasks.All( t => t.Result == null ) );
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