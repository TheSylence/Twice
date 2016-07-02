using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Twitter;

namespace Twice.Tests.Models.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class TweetSourceTests
	{
		[TestMethod, TestCategory( "Models.Twitter" )]
		public void AppIsCorrectlyRecognized()
		{
			// Arrange
			const string url = "<a href=\"http://example.com\" rel=\"nofollow\">ExampleApp</a>";

			// Act
			var source = new TweetSource( url );

			// Assert
			Assert.AreEqual( "ExampleApp", source.Name );
			Assert.AreEqual( new Uri( "http://example.com" ), source.Url );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void EmptySourceUsesWebAsFallback()
		{
			// Arrange
			var url = string.Empty;

			// Act
			var source = new TweetSource( url );

			// Assert
			Assert.AreEqual( "web", source.Name );
			Assert.AreEqual( new Uri( "https://twitter.com" ), source.Url );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void MalformedSourceThrows()
		{
			// Arrange
			const string url = "this is a test";

			// Act
			// ReSharper disable once ObjectCreationAsStatement
			var ex = ExceptionAssert.Catch<ArgumentException>( () => new TweetSource( url ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void NullSourceThrowsException()
		{
			// Arrange Act
			// ReSharper disable once ObjectCreationAsStatement
			var ex = ExceptionAssert.Catch<ArgumentNullException>( () => new TweetSource( null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void StrangeFormattedAppsAreRecognized()
		{
			// Arrange
			const string wrongOrderUrl = "<a rel=\"nofollow\" href=\"http://example.com\">ExampleApp</a>";
			const string noRelUrl = "<a href=\"http://example.com\">ExampleApp</a>";

			// Act
			var wrongOrderSource = new TweetSource( wrongOrderUrl );
			var noRelSource = new TweetSource( noRelUrl );

			// Assert
			Assert.AreEqual( "ExampleApp", wrongOrderSource.Name );
			Assert.AreEqual( new Uri( "http://example.com" ), wrongOrderSource.Url );
			Assert.AreEqual( "ExampleApp", noRelSource.Name );
			Assert.AreEqual( new Uri( "http://example.com" ), noRelSource.Url );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public void WebIsCorrectlyRecognized()
		{
			// Arrange
			const string url = "web";

			// Act
			var source = new TweetSource( url );

			// Assert
			Assert.AreEqual( "web", source.Name );
			Assert.AreEqual( new Uri( "https://twitter.com" ), source.Url );
		}
	}
}