using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;

namespace Twice.Tests.Models.Twitter.Streaming
{
	[TestClass, ExcludeFromCodeCoverage]
	public class StreamingRepositoryTests
	{
		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ParsersAreReused()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();

			var parser = new Mock<IStreamParser>();
			parser.Setup( p => p.Dispose() ).Verifiable();

			var repo = new TestStreamingRepository( contextList.Object );
			repo.InjectStream( 123, parser.Object );

			// Act
			var second = repo.GetParser( new ColumnDefinition( ColumnType.User ) { SourceAccounts = new ulong[] { 123 } } );

			// Assert
			Assert.AreSame( parser.Object, second );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void StreamsAreDisposedWhenRepositoryIs()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();

			var parser = new Mock<IStreamParser>();
			parser.Setup( p => p.Dispose() ).Verifiable();

			var repo = new TestStreamingRepository( contextList.Object );
			repo.InjectStream( 123, parser.Object );

			// Act
			repo.Dispose();

			// Assert
			parser.Verify( p => p.Dispose(), Times.Once() );
		}

		private class TestStreamingRepository : StreamingRepository
		{
			public TestStreamingRepository( ITwitterContextList contextList )
				: base( contextList )
			{
			}

			public void InjectStream( ulong id, IStreamParser parser )
			{
				LoadedParsers.Add( new ParserKey( id, LinqToTwitter.StreamingType.User ), parser );
			}
		}
	}
}