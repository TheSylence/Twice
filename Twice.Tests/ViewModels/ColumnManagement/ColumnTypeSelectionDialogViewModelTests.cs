using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Models.Columns;
using Twice.ViewModels.ColumnManagement;

namespace Twice.Tests.ViewModels.ColumnManagement
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ColumnTypeSelectionDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var vm = new ColumnTypeSelectionDialogViewModel();
			var tester = new PropertyChangedTester( vm );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void OnlyPredefinedColumnsAreShown()
		{
			// Arrange
			var vm = new ColumnTypeSelectionDialogViewModel();

			// Act
			var types = vm.AvailableColumnTypes.Select( c => c.Content.Type ).ToArray();

			// Assert
			var expected = new[] {ColumnType.Mentions, ColumnType.Timeline, ColumnType.Messages};
			CollectionAssert.AreEquivalent( expected, types );
		}
	}
}