using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Media;

namespace Twice.Tests.Models.Media
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MediaExtractorRepositoryTests
	{
		[TestMethod, TestCategory( "Models.Media" )]
		public void AllExtractorsAreChecked()
		{
			// Arrange
			var repository = new MediaExtractorRepository();
			var e1 = new Mock<IMediaExtractor>();
			e1.Setup( e => e.CanExtract( "test" ) ).Returns( false );
			var e2 = new Mock<IMediaExtractor>();
			e2.Setup( e => e.CanExtract( "test" ) ).Returns( false );

			repository.AddExtractor( e1.Object );
			repository.AddExtractor( e2.Object );

			// Act
			repository.ExtractMedia( "test" );

			// Assert
			e1.Verify( v => v.CanExtract( "test" ), Times.Once() );
			e2.Verify( v => v.CanExtract( "test" ), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void NonMatchingExtractorIsNotUsed()
		{
			// Arrange
			var repository = new MediaExtractorRepository();
			var extractor = new Mock<IMediaExtractor>();
			extractor.Setup( v => v.CanExtract( "test" ) ).Returns( false );
			extractor.Setup( v => v.GetMediaUrl( "test" ) ).Verifiable();

			repository.AddExtractor( extractor.Object );

			// Act
			repository.ExtractMedia( "test" );

			// Assert
			extractor.Verify( v => v.CanExtract( "test" ), Times.Once() );
			extractor.Verify( v => v.GetMediaUrl( "test" ), Times.Never() );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void NullIsReturnedWhenNoExtractorWasFound()
		{
			// Arrange
			var repository = new MediaExtractorRepository();

			// Act
			var url = repository.ExtractMedia( "test" );

			// Assert
			Assert.IsNull( url );
		}

		[TestMethod, TestCategory( "Models.Media" )]
		public void SearchEndsWithFirstMatch()
		{
			// Arrange
			var repository = new MediaExtractorRepository();
			var e1 = new Mock<IMediaExtractor>();
			e1.Setup( e => e.CanExtract( "test" ) ).Returns( true );
			var e2 = new Mock<IMediaExtractor>();
			e2.Setup( e => e.CanExtract( "test" ) ).Returns( true );

			repository.AddExtractor( e1.Object );
			repository.AddExtractor( e2.Object );

			// Act
			repository.ExtractMedia( "test" );

			// Assert
			e1.Verify( v => v.CanExtract( "test" ), Times.Once() );
			e2.Verify( v => v.CanExtract( "test" ), Times.Never() );
		}
	}
}