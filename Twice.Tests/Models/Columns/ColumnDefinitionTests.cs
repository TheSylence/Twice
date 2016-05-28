using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Columns;

namespace Twice.Tests.Models.Columns
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ColumnDefinitionTests
	{
		[TestMethod, TestCategory( "Models.Columns" )]
		public void EqualsWorksAsExpected()
		{
			// Arrange
			var a = new ColumnDefinition( ColumnType.Timeline )
			{
				SourceAccounts = new ulong[] { 1, 2, 3 },
				TargetAccounts = new ulong[] { 4, 5, 6 }
			};

			var b = new ColumnDefinition( ColumnType.Timeline )
			{
				SourceAccounts = new ulong[] { 1, 2, 3 },
				TargetAccounts = new ulong[] { 4, 5, 6 }
			};

			var c = new ColumnDefinition( ColumnType.Timeline )
			{
				SourceAccounts = new ulong[] { 4, 5, 6 },
				TargetAccounts = new ulong[] { 4, 5, 6 }
			};

			var d = new ColumnDefinition( ColumnType.Timeline )
			{
				SourceAccounts = new ulong[] { 1, 2, 3 },
				TargetAccounts = new ulong[] { 1, 2, 3 }
			};

			var e = new ColumnDefinition( ColumnType.Activity )
			{
				SourceAccounts = new ulong[] { 1, 2, 3 },
				TargetAccounts = new ulong[] { 4, 5, 6 }
			};

			// Act
			bool equal = a.Equals( b );
			bool equalRev = b.Equals( a );

			var differentSource = a.Equals( c );
			var differentSourceRev = c.Equals( a );

			var differentTarget = a.Equals( d );
			var differentTargetRev = d.Equals( a );

			var differentType = a.Equals( e );
			var differentTypeRev = e.Equals( a );

			// Assert
			Assert.IsTrue( equal );
			Assert.IsTrue( equalRev );
			Assert.IsFalse( differentSource );
			Assert.IsFalse( differentSourceRev );
			Assert.IsFalse( differentTarget );
			Assert.IsFalse( differentTargetRev );
			Assert.IsFalse( differentType );
			Assert.IsFalse( differentTypeRev );
		}

		[TestMethod, TestCategory( "Models.Columns" )]
		public void GetHashCodeReturnsValuesAsDocumented()
		{
			// Arrange
			var a = new ColumnDefinition( ColumnType.Timeline )
			{
				SourceAccounts = new ulong[] { 1, 2, 3 },
				TargetAccounts = new ulong[] { 4, 5, 6 }
			};

			var b = new ColumnDefinition( ColumnType.Timeline )
			{
				SourceAccounts = new ulong[] { 1, 2, 3 },
				TargetAccounts = new ulong[] { 4, 5, 6 }
			};

			var c = new ColumnDefinition( ColumnType.Timeline )
			{
				SourceAccounts = new ulong[] { 3, 2, 1 },
				TargetAccounts = new ulong[] { 4, 5, 6 }
			};

			var d = new ColumnDefinition( ColumnType.Timeline )
			{
				SourceAccounts = new ulong[] { 1, 2, 3 },
				TargetAccounts = new ulong[] { 6, 5, 4 }
			};

			var e = new ColumnDefinition( ColumnType.Activity )
			{
				SourceAccounts = new ulong[] { 1, 2, 3 },
				TargetAccounts = new ulong[] { 4, 5, 6 }
			};

			// Act
			var hashA = a.GetHashCode();
			var hashB = b.GetHashCode();
			var hashC = c.GetHashCode();
			var hashD = d.GetHashCode();
			var hashE = e.GetHashCode();

			// Assert
			Assert.AreEqual( hashA, hashB );
			Assert.AreEqual( hashC, hashA );
			Assert.AreEqual( hashD, hashA );
			Assert.AreNotEqual( hashA, hashE );
		}
	}
}