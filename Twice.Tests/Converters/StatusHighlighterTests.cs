using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Windows.Documents;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass]
	public class StatusHighlighterTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new StatusHighlighter();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void EntitiesAtEndAreCorrectlyEmbedded()
		{
			// Arrange
			var conv = new StatusHighlighter();
			var status = DummyGenerator.CreateDummyStatus();
			status.Entities.UserMentionEntities.Add( new LinqToTwitter.UserMentionEntity
			{
				ScreenName = "Testi",
				Name = "Test name",
				Start = "This is a test ".Length,
				End = "This is a test @Testi".Length
			} );
			status.Text = "This is a test @Testi";

			// Act
			var inlines = (Inline[])conv.Convert( status, null, null, null );

			// Assert
			Assert.AreEqual( 2, inlines.Length );

			Assert.IsInstanceOfType( inlines[1], typeof( Hyperlink ) );
			var linkInlines = ( (Hyperlink)inlines[1] ).Inlines.ToArray();
			Assert.AreEqual( "@Testi", ( (Run)linkInlines[0] ).Text );

			Assert.IsInstanceOfType( inlines[0], typeof( Run ) );
			Assert.AreEqual( "This is a test ", ( (Run)inlines[0] ).Text );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void EntitiesAtStartAreCorrectlyEmbedded()
		{
			// Arrange
			var conv = new StatusHighlighter();
			var status = DummyGenerator.CreateDummyStatus();
			status.Entities.UserMentionEntities.Add( new LinqToTwitter.UserMentionEntity
			{
				ScreenName = "Testi",
				Name = "Test name",
				Start = 0,
				End = "Testi".Length + 1
			} );
			status.Text = "@Testi This is a test";

			// Act
			var inlines = (Inline[])conv.Convert( status, null, null, null );

			// Assert
			Assert.AreEqual( 2, inlines.Length );

			Assert.IsInstanceOfType( inlines[0], typeof( Hyperlink ) );
			var linkInlines = ( (Hyperlink)inlines[0] ).Inlines.ToArray();
			Assert.AreEqual( "@Testi", ( (Run)linkInlines[0] ).Text );

			Assert.IsInstanceOfType( inlines[1], typeof( Run ) );
			Assert.AreEqual( " This is a test", ( (Run)inlines[1] ).Text );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void HtmlCharsAreDecodedCorrectly()
		{
			// Arrange
			const string content = "<test>";
			var conv = new StatusHighlighter();
			var status = DummyGenerator.CreateDummyStatus();
			status.Text = WebUtility.HtmlEncode( content );

			// Act
			var inlines = (Inline[])conv.Convert( status, null, null, null );

			// Assert
			Assert.AreEqual( 1, inlines.Length );
			Assert.IsInstanceOfType( inlines[0], typeof( Run ) );
			Assert.AreEqual( content, ( (Run)inlines[0] ).Text );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NonStatusThrowsException()
		{
			// Arrange
			var conv = new StatusHighlighter();

			// Act
			var ex = ExceptionAssert.Catch<ArgumentException>( () => conv.Convert( string.Empty, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void SimpleTextStatusResultsInSingeInline()
		{
			// Arrange
			var conv = new StatusHighlighter();
			var status = DummyGenerator.CreateDummyStatus();
			status.Text = "Hello World";

			// Act
			var inlines = (Inline[])conv.Convert( status, null, null, null );

			// Assert
			Assert.AreEqual( 1, inlines.Length );
			Assert.IsInstanceOfType( inlines[0], typeof( Run ) );
			Assert.AreEqual( status.Text, ( (Run)inlines[0] ).Text );
		}
	}
}