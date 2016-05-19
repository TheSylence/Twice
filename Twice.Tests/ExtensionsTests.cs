using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twice.Tests
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ExtensionsTests
	{
		[TestMethod, TestCategory( "Extensions" )]
		public void EnumerableCompareWorks()
		{
			// Arrange
			var a = new[] {1, 2, 3};
			var b = new[] {1, 2, 3};
			var c = new[] {3, 2, 1};
			var d = new[] {1, 2};
			var e = new[] {2, 3};
			var f = new[] {1, 2, 3, 4};
			var g = new[] {5, 6, 7};
			var h = new List<int> {8, 9, 2, 3, 1};

			// Act
			var ab = a.Compare( b );
			var ba = b.Compare( a );
			var ac = a.Compare( c );
			var ca = c.Compare( a );
			var ad = a.Compare( d );
			var da = d.Compare( a );
			var ae = a.Compare( e );
			var af = a.Compare( f );
			var ag = a.Compare( g );
			var ah = a.Compare( h );
			var ha = h.Compare( a );

			// Assert
			Assert.IsTrue( ab );
			Assert.IsTrue( ba );
			Assert.IsTrue( ac );
			Assert.IsTrue( ca );
			Assert.IsFalse( ad );
			Assert.IsFalse( da );
			Assert.IsFalse( ae );
			Assert.IsFalse( af );
			Assert.IsFalse( ag );
			Assert.IsFalse( ah );
			Assert.IsFalse( ha );
		}
	}
}