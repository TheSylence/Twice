using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Columns;

namespace Twice.Tests.Models.Columns
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ColumnDefinitionFactoryTests
	{
		[TestMethod, TestCategory( "Models.Columns" )]
		public void ConstructedColumnHasUniqueId()
		{
			// Act
			var c1 = ColumnDefinitionFactory.Construct( ColumnType.Activity, new ulong[] {1}, new ulong[] {2} );
			var c2 = ColumnDefinitionFactory.Construct( ColumnType.Activity, new ulong[] {1}, new ulong[] {2} );

			// Assert
			Assert.AreNotEqual( Guid.Empty, c1.Id );
			Assert.AreNotEqual( Guid.Empty, c2.Id );
			Assert.AreNotEqual( c1.Id, c2.Id );
		}
	}
}