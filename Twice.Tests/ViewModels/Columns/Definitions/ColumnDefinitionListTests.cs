using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Twice.ViewModels.Columns.Definitions;

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
				AccountIds = new ulong[] {1234u, 45678u},
				SourceAccount = 456u
			};

			var timelineDef = new ColumnDefinition( ColumnType.Timeline )
			{
				Width = 223,
				AccountIds = new ulong[] {111u, 222u},
				SourceAccount = 2344u
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
			Assert.AreEqual( mentionDef.SourceAccount, col.SourceAccount );
			CollectionAssert.AreEqual( mentionDef.AccountIds, col.AccountIds );

			col = loaded.SingleOrDefault( c => c.Type == ColumnType.Timeline );
			Assert.IsNotNull( col );
			Assert.AreEqual( timelineDef.Width, col.Width );
			Assert.AreEqual( timelineDef.Type, col.Type );
			Assert.AreEqual( timelineDef.SourceAccount, col.SourceAccount );
			CollectionAssert.AreEqual( timelineDef.AccountIds, col.AccountIds );
		}
	}
}