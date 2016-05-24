using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class CompareTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new Compare();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void EqualIsCheckedCorrectly()
		{
			// Arrange
			var conv = new Compare
			{
				Operation = CompareOperation.Equal,
				CompareValue = 123
			};

			// Act
			var res = conv.Convert( "123", null, null, null );

			// Assert
			Assert.AreEqual( true, res );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void EqualsWithOverrideValueIsCheckedCorrectly()
		{
			// Arrange
			var conv = new Compare
			{
				Operation = CompareOperation.Equal,
				CompareValue = 123
			};

			// Act
			var res = conv.Convert( "222", null, 222, null );

			// Assert
			Assert.AreEqual( true, res );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void GreaterEqualIsCheckedCorrectly()
		{
			// Arrange
			var conv = new Compare
			{
				Operation = CompareOperation.GreaterOrEqual
			};

			// Act
			var eq = conv.Convert( 2, null, 2, null );
			var gt = conv.Convert( 3, null, 2, null );
			var lt = conv.Convert( 1, null, 2, null );

			// Assert
			Assert.AreEqual( true, eq );
			Assert.AreEqual( true, gt );
			Assert.AreEqual( false, lt );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void GreaterIsCheckedCorrectly()
		{
			// Arrange
			var conv = new Compare
			{
				Operation = CompareOperation.Greater,
				CompareValue = 0
			};

			// Act
			var res = conv.Convert( "1", null, null, null );

			// Assert
			Assert.AreEqual( true, res );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void LessEqualIsCheckedCorrectly()
		{
			// Arrange
			var conv = new Compare
			{
				Operation = CompareOperation.LessOrEqual
			};

			// Act
			var eq = conv.Convert( 2, null, 2, null );
			var gt = conv.Convert( 3, null, 2, null );
			var lt = conv.Convert( 1, null, 2, null );

			// Assert
			Assert.AreEqual( true, eq );
			Assert.AreEqual( false, gt );
			Assert.AreEqual( true, lt );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void LessIsCheckedCorrectly()
		{
			// Arrange
			var conv = new Compare
			{
				Operation = CompareOperation.Less
			};

			// Act
			var eq = conv.Convert( 2, null, 2, null );
			var lt = conv.Convert( 1, null, 2, null );

			// Assert
			Assert.AreEqual( false, eq );
			Assert.AreEqual( true, lt );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NonNumericValueReturnsUnsetValue()
		{
			// Arrange
			var conv = new Compare();

			// Act
			var res = conv.Convert( string.Empty, null, null, null );

			// Assert
			Assert.AreEqual( DependencyProperty.UnsetValue, res );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NotEqualIsCheckedCorrectly()
		{
			// Arrange
			var conv = new Compare
			{
				Operation = CompareOperation.NotEqual,
				CompareValue = 2
			};

			// Act
			var res = conv.Convert( 2, null, null, null );

			// Assert
			Assert.AreEqual( false, res );
		}
	}
}