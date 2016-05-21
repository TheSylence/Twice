using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class SourceNameTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new SourceName();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void CorrectSourceNameIsReturned()
		{
			// Arrange
			var conv = new SourceName();

			// Act
			var name = conv.Convert( string.Empty, null, null, null );

			// Assert
			Assert.AreEqual( "web", name );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NullReturnsUnsetValue()
		{
			// Arrange
			var conv = new SourceName();

			// Act
			var v = conv.Convert( null, null, null, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, v );
		}
	}
}