using System;
using System.Diagnostics.CodeAnalysis;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Models.Twitter.Comparers;

namespace Twice.Tests.Models.Twitter.Comparers
{
	[TestClass, ExcludeFromCodeCoverage]
	public class StatusComparerTests
	{
		[TestMethod, TestCategory( "Models.Twitter.Comparers" )]
		public void StatusIsComparedBasedOnId()
		{
			// Arrange
			var comp = new StatusComparer();
			var a = new Status
			{
				ID = 123
			};
			var b = new Status
			{
				ID = 123
			};
			var c = new Status
			{
				ID = 111
			};

			// Act
			var ab = comp.Equals( a, b );
			var ba = comp.Equals( b, a );
			var ac = comp.Equals( a, c );

			// Assert
			Assert.IsTrue( ab );
			Assert.IsTrue( ba );
			Assert.IsFalse( ac );
		}

		[TestMethod, TestCategory( "Models.Twitter.Comparers" )]
		public void HashCodeForNullThrowsException()
		{
			// Arrange
			var comp = new StatusComparer();

			// Act
			var ex = ExceptionAssert.Catch<ArgumentNullException>( () => comp.GetHashCode( null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Models.Twitter.Comparers" )]
		public void HashCodeForObjectIsCalculatedCorrectly()
		{
			// Arrange
			var comp = new StatusComparer();
			var entity = new Status
			{
				ID = 123
			};

			// Act
			var hash = comp.GetHashCode( entity );

			// Assert
			Assert.AreEqual( entity.ID.GetHashCode(), hash );
		}
	}
}