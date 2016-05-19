using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MathConverterTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new MathConverter();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void InvalidParameterReturnsUnsetValue()
		{
			// Arrange
			var conv = new MathConverter {Operation = MathOperation.Add};

			// Act
			var result = conv.Convert( null, null, new object(), null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, result );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void InvalidValueReturnsUnsetValue()
		{
			// Arrange
			var conv = new MathConverter {Operation = MathOperation.Add};

			// Act
			var result = conv.Convert( new object(), null, null, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, result );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NumbersCanBeAdded()
		{
			// Arrange
			var conv = new MathConverter {Operation = MathOperation.Add};

			// Act
			var result = conv.Convert( 12.0, null, 3, null );

			// Assert
			Assert.AreEqual( 15.0, result );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NumbersCanBeDevided()
		{
			// Arrange
			var conv = new MathConverter {Operation = MathOperation.Divide};

			// Act
			var result = conv.Convert( "10.0", null, "5", null );

			// Assert
			Assert.AreEqual( 2.0, result );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NumbersCanBeMultiplied()
		{
			// Arrange
			var conv = new MathConverter {Operation = MathOperation.Multiply};

			// Act
			var result = conv.Convert( 3, null, "2.0", null );

			// Assert
			Assert.AreEqual( 6.0, result );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NumbersCanBeSubstracted()
		{
			// Arrange
			var conv = new MathConverter {Operation = MathOperation.Substract};

			// Act
			var result = conv.Convert( "10", null, 3.0, null );

			// Assert
			Assert.AreEqual( 7.0, result );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void UnknownOperationReturnsUnsetValue()
		{
			// Arrange
			var conv = new MathConverter {Operation = MathOperation.Add + 10};

			// Act
			var result = conv.Convert( 1, null, 2, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, result );
		}
	}
}