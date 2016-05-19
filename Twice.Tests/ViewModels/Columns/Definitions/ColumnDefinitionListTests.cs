using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Columns;
using Twice.Utilities;

namespace Twice.Tests.ViewModels.Columns.Definitions
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ColumnDefinitionListTests
	{
		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void AddingColumnsPreservesExistingOnes()
		{
			// Arrange
			var fileName = Path.GetTempFileName();
			var list = new ColumnDefinitionList( fileName )
			{
				Serializer = new Serializer()
			};

			var mentionDef = new ColumnDefinition( ColumnType.Mentions )
			{
				Width = 123,
				TargetAccounts = new ulong[] {1234u, 45678u},
				SourceAccounts = new ulong[] {456u}
			};

			var timelineDef = new ColumnDefinition( ColumnType.Timeline )
			{
				Width = 223,
				TargetAccounts = new ulong[] {111u, 222u},
				SourceAccounts = new ulong[] {2344u}
			};

			var definitions = new[]
			{
				timelineDef
			};

			list.Save( definitions );

			// Act
			list.AddColumns( new[] {mentionDef} );
			var loaded = list.Load().ToArray();

			// Assert
			Assert.AreEqual( 2, loaded.Length );
			Assert.IsNotNull( loaded.SingleOrDefault( c => c.Type == ColumnType.Mentions ) );
			Assert.IsNotNull( loaded.SingleOrDefault( c => c.Type == ColumnType.Timeline ) );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void DefinitionCanBeRemoved()
		{
			// Arrange
			var fileName = Path.GetTempFileName();
			var list = new ColumnDefinitionList( fileName )
			{
				Serializer = new Serializer()
			};

			list.AddColumns( new[] {new ColumnDefinition( ColumnType.User )} );

			// Act
			var saved = list.Load().ToArray();
			var countBefore = saved.Length;

			list.Remove( saved );

			var countAfter = list.Load().Count();

			// Assert
			Assert.AreNotEqual( 0, countBefore );
			Assert.AreEqual( 0, countAfter );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void LoadingNonExistentFileReturnsEmptyList()
		{
			// Arrange
			string fileName = "non.existing.file";
			var list = new ColumnDefinitionList( fileName );

			// Act
			var loaded = list.Load().ToArray();

			// Assert
			Assert.AreEqual( 0, loaded.Length );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void SavedColumnDefinitionsCanBeLoaded()
		{
			// Arrange
			var mentionDef = new ColumnDefinition( ColumnType.Mentions )
			{
				Width = 123,
				TargetAccounts = new ulong[] {1234u, 45678u},
				SourceAccounts = new ulong[] {456u}
			};

			var timelineDef = new ColumnDefinition( ColumnType.Timeline )
			{
				Width = 223,
				TargetAccounts = new ulong[] {111u, 222u},
				SourceAccounts = new ulong[] {2344u}
			};

			var definitions = new[]
			{
				mentionDef,
				timelineDef
			};

			var fileName = Path.GetTempFileName();
			var list = new ColumnDefinitionList( fileName )
			{
				Serializer = new Serializer()
			};

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
			var serializer = new Mock<ISerializer>();
			var list = new ColumnDefinitionList( fileName )
			{
				Serializer = serializer.Object
			};
			bool raised = false;
			list.ColumnsChanged += ( s, e ) => raised = true;

			// Act
			list.Save( Enumerable.Empty<ColumnDefinition>() );

			// Assert
			Assert.IsTrue( raised );
		}
	}
}