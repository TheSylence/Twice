using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using Twice.Converters;
using TestCaseKey = System.Tuple<object, object, Twice.Converters.CompareOperation>;

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
		public void ExoticCompareTestCases()
		{
			// Arrange
			var conv = new Compare();

			var testCases = new Dictionary<TestCaseKey, object>
			{
				{new TestCaseKey( 1.0, 1.0, CompareOperation.Equal ), true},
				{new TestCaseKey( 1.1, "test", CompareOperation.Greater ), DependencyProperty.UnsetValue},
				{new TestCaseKey( 0, 0, (CompareOperation)int.MaxValue ), DependencyProperty.UnsetValue}
			};

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp =>
			{
				conv.Operation = kvp.Key.Item3;
				return conv.Convert( kvp.Key.Item1, null, kvp.Key.Item2, null );
			} );

			// Assert
			foreach( var kvp in results )
			{
				var exp = testCases[kvp.Key];

				Assert.AreEqual( exp, kvp.Value );
			}
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