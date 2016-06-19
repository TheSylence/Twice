using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Models.Twitter.Entities;

namespace Twice.Tests.Models.Twitter.Entities
{
	[TestClass]
	public class EntityParserTests
	{
		[TestMethod, TestCategory( "Models.Twitter.Entities" )]
		public void DuplicateHashtagIsFoundTwice()
		{
			// Arrange
			string text = "#test #test";

			// Act
			var hashtags = EntityParser.ExtractHashtags( text );

			// Assert
			Assert.AreEqual( 2, hashtags.Count );
			Assert.IsTrue( hashtags.All( t => t.Tag == "test" ) );
		}

		[TestMethod, TestCategory( "Models.Twitter.Entities" )]
		public void HashatagAtEndIsCorrectlyExtracted()
		{
			// Arrange
			string text = "Hello #World";

			// Act
			var hashtags = EntityParser.ExtractHashtags( text );

			// Assert
			Assert.AreEqual( 1, hashtags.Count );

			var tag = hashtags.First();
			Assert.AreEqual( 6, tag.Start );
			Assert.AreEqual( 12, tag.End );
			Assert.AreEqual( "World", tag.Tag );
		}

		[TestMethod, TestCategory( "Models.Twitter.Entities" )]
		public void HashtagAtStartIsCorrectlyExtracted()
		{
			// Arrange
			string text = "#Hello World!";

			// Act
			var hashtags = EntityParser.ExtractHashtags( text );

			// Assert
			Assert.AreEqual( 1, hashtags.Count );

			var tag = hashtags.First();
			Assert.AreEqual( 0, tag.Start );
			Assert.AreEqual( 6, tag.End );
			Assert.AreEqual( "Hello", tag.Tag );
		}

		[TestMethod, TestCategory( "Models.Twitter.Entities" )]
		public void HashtagsInMiddleAreCorrectlyExtracted()
		{
			// Arrange
			string text = "Hello #World this is a test";

			// Act
			var hashtags = EntityParser.ExtractHashtags( text );

			// Assert
			Assert.AreEqual( 1, hashtags.Count );

			var tag = hashtags.First();
			Assert.AreEqual( 6, tag.Start );
			Assert.AreEqual( 12, tag.End );
			Assert.AreEqual( "World", tag.Tag );
		}

		[TestMethod, TestCategory( "Models.Twitter.Entities" )]
		public void MultipleHashtagsAreCorrectlyExtracted()
		{
			// Arrange
			string text = "Hello #World this #is a test";

			// Act
			var hashtags = EntityParser.ExtractHashtags( text );

			// Assert
			Assert.AreEqual( 2, hashtags.Count );

			var tag = hashtags.First();
			Assert.AreEqual( 6, tag.Start );
			Assert.AreEqual( 12, tag.End );
			Assert.AreEqual( "World", tag.Tag );

			tag = hashtags.Last();
			Assert.AreEqual( 18, tag.Start );
			Assert.AreEqual( 21, tag.End );
			Assert.AreEqual( "is", tag.Tag );
		}
	}
}