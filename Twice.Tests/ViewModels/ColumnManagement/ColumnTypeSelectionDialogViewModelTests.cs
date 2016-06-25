using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Models.Columns;
using Twice.ViewModels.ColumnManagement;

namespace Twice.Tests.ViewModels.ColumnManagement
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ColumnTypeSelectionDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void DeselectingSingleTypeTogglesSelectAll()
		{
			// Arrange
			var vm = new ColumnTypeSelectionDialogViewModel();

			// Act
			vm.AvailableColumnTypes.First().IsSelected = false;
			bool deselected = vm.SelectAll;

			bool other = vm.AvailableColumnTypes.Skip( 1 ).All( c => c.IsSelected );

			vm.AvailableColumnTypes.First().IsSelected = true;
			bool selected = vm.SelectAll;

			// Assert
			Assert.IsFalse( deselected );
			Assert.IsTrue( other );
			Assert.IsTrue( selected );
		}

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

		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void SelectAllTogglesAllTypes()
		{
			// Arrange
			var vm = new ColumnTypeSelectionDialogViewModel();

			// Act
			bool before = vm.AvailableColumnTypes.All( c => c.IsSelected );

			vm.SelectAll = false;
			bool none = vm.AvailableColumnTypes.All( c => c.IsSelected );

			vm.SelectAll = true;
			bool all = vm.AvailableColumnTypes.All( c => c.IsSelected );

			// Assert
			Assert.IsTrue( before );
			Assert.IsFalse( none );
			Assert.IsTrue( all );
		}
	}
}