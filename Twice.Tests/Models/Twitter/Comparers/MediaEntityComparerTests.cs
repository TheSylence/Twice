using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Twitter.Comparers;

namespace Twice.Tests.Models.Twitter.Comparers
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MediaEntityComparerTests
	{
		[TestMethod, TestCategory( "Models.Twitter.Comparers" )]
		public void EntityIsComparedBasedOnId()
		{
			// Arrange
			var comp = new MediaEntityComparer();
			var a = new MediaEntity
			{
				ID = 123
			};
			var b = new MediaEntity
			{
				ID = 123
			};
			var c = new MediaEntity
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
			var comp = new MediaEntityComparer();

			// Act
			// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
			var ex = ExceptionAssert.Catch<ArgumentNullException>( () => comp.GetHashCode( null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Models.Twitter.Comparers" )]
		public void HashCodeForObjectIsCalculatedCorrectly()
		{
			// Arrange
			var comp = new MediaEntityComparer();
			var entity = new MediaEntity
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