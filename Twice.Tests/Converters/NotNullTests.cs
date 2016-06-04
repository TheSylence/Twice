using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class NotNullTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new NotNull();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NullCheckIsDoneCorrectly()
		{
			// Arrange
			var conv = new NotNull();

			// Act
			bool nullObject = (bool)conv.Convert( null, null, null, null );
			bool valueType = (bool)conv.Convert( 123, null, null, null );
			bool refType = (bool)conv.Convert( string.Empty, null, null, null );

			// Assert
			Assert.IsFalse( nullObject );
			Assert.IsTrue( valueType );
			Assert.IsTrue( refType );
		}
	}
}