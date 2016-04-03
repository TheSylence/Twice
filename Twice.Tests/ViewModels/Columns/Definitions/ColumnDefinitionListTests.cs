using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Models.Columns;

namespace Twice.Tests.ViewModels.Columns.Definitions
{
	[TestClass]
	public class ColumnDefinitionListTests
	{
		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void SavedColumnDefinitionsCanBeLoaded()
		{
			// Arrange
			var mentionDef = new ColumnDefinition( ColumnType.Mentions )
			{
				Width = 123,
				TargetAccounts = new ulong[] {1234u, 45678u},
				SourceAccounts = new ulong[] { 456u }
			};

			var timelineDef = new ColumnDefinition( ColumnType.Timeline )
			{
				Width = 223,
				TargetAccounts = new ulong[] {111u, 222u},
				SourceAccounts = new ulong[] { 2344u }
			};

			var definitions = new[]
			{
				mentionDef,
				timelineDef
			};

			var fileName = Path.GetTempFileName();
			var list = new ColumnDefinitionList( fileName );

			// Act
			list.Save( definitions );
			var loaded = list.Load().ToArray();

			// Assert
			var col = loaded.SingleOrDefault( c => c.Type == ColumnType.Mentions );
			Assert.IsNotNull( col );
			Assert.AreEqual( mentionDef.Width, col.Width );
			Assert.AreEqual( mentionDef.Type, col.Type );
			CollectionAssert.AreEqual( mentionDef.SourceAccounts, col.SourceAccounts );
			CollectionAssert.AreEqual( mentionDef.TargetAccounts, col.TargetAccounts );

			col = loaded.SingleOrDefault( c => c.Type == ColumnType.Timeline );
			Assert.IsNotNull( col );
			Assert.AreEqual( timelineDef.Width, col.Width );
			Assert.AreEqual( timelineDef.Type, col.Type );
			CollectionAssert.AreEqual( timelineDef.SourceAccounts, col.SourceAccounts );
			CollectionAssert.AreEqual( timelineDef.TargetAccounts, col.TargetAccounts );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void SavingColumnsRaisesChangeEvent()
		{
			// Arrange
			var fileName = Path.GetTempFileName();
			var list = new ColumnDefinitionList( fileName );
			bool raised = false;
			list.ColumnsChanged += ( s, e ) => raised = true;

			// Act
			list.Save( Enumerable.Empty<ColumnDefinition>() );

			// Assert
			Assert.IsTrue( raised );
		}
	}
}