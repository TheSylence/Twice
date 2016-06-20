using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class StatusLinkTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new StatusLink();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void CorrectUrlIsExctracted()
		{
			// Arrange
			var conv = new StatusLink();
			var status = DummyGenerator.CreateDummyStatus();
			status.StatusID = 123;
			status.User.ScreenName = "username";

			// Act
			var url = (Uri)conv.Convert( status, null, null, null );

			// Assert
			Assert.AreEqual( "https://twitter.com/username/status/123", url.AbsoluteUri );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NonStatusReturnsItself()
		{
			// Arrange
			var conv = new StatusLink();

			// Act
			var result = conv.Convert( "test", null, null, null );

			// Assert
			Assert.AreEqual( "test", result );
		}
	}
}