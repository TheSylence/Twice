using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	[SuppressMessage( "ReSharper", "PossibleNullReferenceException" )]
	public class EmptyStringTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new EmptyString();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NonStringIsHandledCorrectly()
		{
			// Arrange
			var conv = new EmptyString();

			// Act
			var nullObj = conv.Convert( null, null, null, null );
			var integer = conv.Convert( 123, null, null, null );

			// Assert
			Assert.IsTrue( (bool)nullObj );
			Assert.IsTrue( (bool)integer );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void StringIsHandledCorrectly()
		{
			// Arrange
			var conv = new EmptyString();

			// Act
			var empty = (bool)conv.Convert( string.Empty, null, null, null );
			var nonEmpty = (bool)conv.Convert( " ", null, null, null );

			// Assert
			Assert.IsTrue( empty );
			Assert.IsFalse( nonEmpty );
		}
	}
}