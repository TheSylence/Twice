using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Utilities;

namespace Twice.Tests.Utilities
{
	[TestClass, ExcludeFromCodeCoverage]
	public class SmartCollectionTests
	{
		[TestMethod, TestCategory( "Utilities" )]
		public void CollectionCanBeConstructedFromEnumerable()
		{
			// Arrange
			var enumerable = Enumerable.Range( 0, 5 );

			// Act
			var collection = new SmartCollection<int>( enumerable );

			// Assert
			CollectionAssert.AreEqual( enumerable.ToArray(), collection.ToArray() );
		}

		[TestMethod, TestCategory( "Utilities" )]
		public void ResetReplacesContent()
		{
			// Arrange
			var collection = new SmartCollection<int>
			{
				1,2,3
			};

			// Act
			collection.Reset( new[] { 4, 5, 6 } );

			// Assert
			CollectionAssert.AreEqual( new[] { 4, 5, 6 }, collection.ToArray() );
		}

		[TestMethod, TestCategory( "Utilities" )]
		public void CollectionCanBeConstructedFromList()
		{
			// Arrange
			var list = new List<int> {3, 6, 3, 2};

			// Act
			var collection = new SmartCollection<int>( list );

			// Assert
			CollectionAssert.AreEqual( list.ToArray(), collection.ToArray() );
		}

		[TestMethod, TestCategory( "Utilities" )]
		public void EmptyConstructorCreatesEmptyList()
		{
			// Arrange Act
			var collection = new SmartCollection<int>();

			// Assert
			Assert.AreEqual( 0, collection.Count );
		}
	}
}