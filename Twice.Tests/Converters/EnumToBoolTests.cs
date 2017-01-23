using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class EnumToBoolTests
	{
		private enum TestEnum
		{
			First = 0,
			Last = 1
		}

		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackReturnsCorrectResult()
		{
			// Arrange
			var conv = new EnumToBool();

			// Act
			var value = conv.ConvertBack( true, typeof( TestEnum ), "First", null );

			// Assert
			Assert.AreEqual( TestEnum.First, value );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackWithInvalidParameterReturnsUnsetValue()
		{
			// Arrange
			var conv = new EnumToBool();

			// Act
			var value = conv.ConvertBack( true, null, 123, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, value );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void ConvertWithInvalidParameterReturnsUnsetValue()
		{
			// Arrange
			var conv = new EnumToBool();

			// Act
			var value = conv.Convert( null, null, 123, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, value );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void ConvertWithInvalidValueReturnsUnsetValue()
		{
			// Arrange
			var conv = new EnumToBool();

			// Act
			var type = conv.Convert( 123, null, string.Empty, null );
			var enumValue = conv.Convert( (TestEnum)2, null, string.Empty, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, type );
			Assert.AreEqual( DependencyProperty.UnsetValue, enumValue );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void MatchedEnumValueReturnsTrue()
		{
			// Arrange
			var conv = new EnumToBool();

			// Act
			var value = (bool)conv.Convert( TestEnum.First, null, "First", null );

			// Assert
			Assert.IsTrue( value );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NotMatchedEnumValueReturnsFalse()
		{
			// Arrange
			var conv = new EnumToBool();

			// Act
			var value = (bool)conv.Convert( TestEnum.Last, null, "First", null );

			// Assert
			Assert.IsFalse( value );
		}
	}
}