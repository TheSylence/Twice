using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class InvertBoolTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void BoolIsInverted()
		{
			// Arrange
			var conv = new InvertBool();

			// Act
			bool convertedTrue = (bool)conv.Convert( true, null, null, null );
			bool convertedFalse = (bool)conv.Convert( false, null, null, null );
			bool backConvertedTrue = (bool)conv.ConvertBack( true, null, null, null );
			bool backConvertedFalse = (bool)conv.ConvertBack( false, null, null, null );

			// Assert
			Assert.IsFalse( convertedTrue );
			Assert.IsTrue( convertedFalse );
			Assert.IsFalse( backConvertedTrue );
			Assert.IsTrue( backConvertedFalse );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void InvalidTypeResultsInUnsetValue()
		{
			// Arrange
			var conv = new InvertBool();

			// Act
			var result = conv.Convert( string.Empty, null, null, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, result );
		}
	}
}