using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MnemonicsRemoverTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new MnemonicsRemover();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void MnemoicsAreRemoved()
		{
			// Arrange
			var conv = new MnemonicsRemover();

			// Act
			var result = conv.Convert( "this_is_a_test", null, null, null );

			// Assert
			Assert.AreEqual( "this__is__a__test", result );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NonStringIsNotTouched()
		{
			// Arrange
			var conv = new MnemonicsRemover();

			// Act
			var result = conv.Convert( 123, null, null, null );

			// Assert
			Assert.AreEqual( 123, result );
		}
	}
}