using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Converters;

namespace Twice.Tests.Converters
{
	[TestClass]
	public class EmptyListTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new EmptyList();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void EmptyListWorksWithArray()
		{
			// Arrange
			var conv = new EmptyList();

			// Act
			var emptyString = (bool)conv.Convert( new string[0], null, null, null );
			var emptyInt = (bool)conv.Convert( new int[0], null, null, null );

			var nonEmptyString = (bool)conv.Convert( new[] { "test" }, null, null, null );
			var nonEmptyInt = (bool)conv.Convert( new[] { 1, 2 }, null, null, null );

			// Assert
			Assert.IsTrue( emptyString );
			Assert.IsTrue( emptyInt );

			Assert.IsFalse( nonEmptyString );
			Assert.IsFalse( nonEmptyInt );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void EmptyListWorksWithEnumerable()
		{
			// Arrange
			var conv = new EmptyList();

			// Act
			var emptyString = (bool)conv.Convert( Enumerable.Empty<string>(), null, null, null );
			var emptyInt = (bool)conv.Convert( Enumerable.Empty<int>(), null, null, null );

			var nonEmptyString = (bool)conv.Convert( Enumerable.Repeat( "test", 2 ), null, null, null );
			var nonEmptyInt = (bool)conv.Convert( Enumerable.Range( 0, 2 ), null, null, null );

			// Assert
			Assert.IsTrue( emptyString );
			Assert.IsTrue( emptyInt );

			Assert.IsFalse( nonEmptyString );
			Assert.IsFalse( nonEmptyInt );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void EmptyListWorksWithList()
		{
			// Arrange
			var conv = new EmptyList();

			// Act
			var emptyString = (bool)conv.Convert( new List<string>(), null, null, null );
			var emptyInt = (bool)conv.Convert( new List<int>(), null, null, null );

			var nonEmptyString = (bool)conv.Convert( new List<string> { "test" }, null, null, null );
			var nonEmptyInt = (bool)conv.Convert( new List<int> { 1, 2 }, null, null, null );

			// Assert
			Assert.IsTrue( emptyString );
			Assert.IsTrue( emptyInt );

			Assert.IsFalse( nonEmptyString );
			Assert.IsFalse( nonEmptyInt );
		}
	}
}