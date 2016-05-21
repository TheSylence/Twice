using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class SourceUrlTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new SourceUrl();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void CorrectUrlIsConverted()
		{
			// Arrange
			var conv = new SourceUrl();

			// Act
			var url = conv.Convert( string.Empty, null, null, null );

			// Assert
			Assert.AreEqual( new Uri( "https://twitter.com" ), url );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NullReturnsUnsetValue()
		{
			// Arrange
			var conv = new SourceUrl();

			// Act
			var v = conv.Convert( null, null, null, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, v );
		}
	}
}