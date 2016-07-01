using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Models.Twitter.Entities;

namespace Twice.Tests.Models.Twitter.Entities
{
	[TestClass, ExcludeFromCodeCoverage]
	public class EntityParserTests
	{
		[TestMethod, TestCategory( "Models.Twitter.Entities" )]
		public void DuplicateHashtagIsFoundTwice()
		{
			// Arrange
			const string text = "#test #test";

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
			const string text = "Hello #World";

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
			const string text = "#Hello World!";

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
			const string text = "Hello #World this is a test";

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
		public void MentionAtEndIsCorrectlyExtracted()
		{
			// Arrange
			const string text = "This is a @test";

			// Act
			var mentions = EntityParser.ExtractMentions( text );

			// Assert
			Assert.AreEqual( 1, mentions.Count );

			var m = mentions.First();
			Assert.AreEqual( 10, m.Start );
			Assert.AreEqual( 15, m.End );
			Assert.AreEqual( "test", m.ScreenName );
		}

		[TestMethod, TestCategory( "Models.Twitter.Entities" )]
		public void MentionAtStartIsCorrectlyExtracted()
		{
			// Arrange
			const string text = "@username Hello";

			// Act
			var mentions = EntityParser.ExtractMentions( text );

			// Assert
			Assert.AreEqual( 1, mentions.Count );

			var m = mentions.First();
			Assert.AreEqual( 0, m.Start );
			Assert.AreEqual( 9, m.End );
			Assert.AreEqual( "username", m.ScreenName );
		}

		[TestMethod, TestCategory( "Models.Twitter.Entities" )]
		public void MentionInMiddleIsExtractedCorrectly()
		{
			// Arrange
			const string text = "Hello @World how are you?";

			// Act
			var mentions = EntityParser.ExtractMentions( text );

			// Assert
			Assert.AreEqual( 1, mentions.Count );

			var m = mentions.First();
			Assert.AreEqual( 6, m.Start );
			Assert.AreEqual( 12, m.End );
			Assert.AreEqual( "World", m.ScreenName );
		}

		[TestMethod, TestCategory( "Models.Twitter.Entities" )]
		public void MultipleHashtagsAreCorrectlyExtracted()
		{
			// Arrange
			const string text = "Hello #World this #is a test";

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

		[TestMethod, TestCategory( "Models.Twitter.Entities" )]
		public void MultipleMentionsAreCorrectlyExtracted()
		{
			// Arrange
			const string text = "@user @test Hello";

			// Act
			var mentions = EntityParser.ExtractMentions( text );

			// Assert
			Assert.AreEqual( 2, mentions.Count );

			var m = mentions.First();
			Assert.AreEqual( 0, m.Start );
			Assert.AreEqual( 5, m.End );
			Assert.AreEqual( "user", m.ScreenName );

			m = mentions.Last();
			Assert.AreEqual( 6, m.Start );
			Assert.AreEqual( 11, m.End );
			Assert.AreEqual( "test", m.ScreenName );
		}
	}
}