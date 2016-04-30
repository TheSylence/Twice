using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Twice.Models.Columns;
using Twice.ViewModels.ColumnManagement;

namespace Twice.Tests.ViewModels.ColumnManagement
{
	[TestClass]
	public class ColumnTypeListFactoryTests
	{
		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void AllColumnTypesCanBeConstructed()
		{
			// Arrange
			var expectedTypes = new[] { ColumnType.Favorites, ColumnType.Mentions, ColumnType.Messages, ColumnType.Timeline, ColumnType.User };

			// Act
			var constructed = ColumnTypeListFactory.GetItems().Select( c => c.Type ).ToArray();

			// Assert
			CollectionAssert.AreEquivalent( expectedTypes, constructed );
		}
	}
}