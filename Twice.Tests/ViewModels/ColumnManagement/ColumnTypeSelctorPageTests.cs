using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Columns;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Wizards;

namespace Twice.Tests.ViewModels.ColumnManagement
{
	[TestClass]
	public class ColumnTypeSelctorPageTests
	{
		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void CorrectColumnTypesAreConstructed()
		{
			// Arrange
			var wizard = new Mock<IWizardViewModel>();

			// Act
			var page = new ColumnTypeSelctorPage( wizard.Object );

			// Assert
			var columnTypes = page.ColumnTypes.Select( c => c.Type ).ToArray();

			CollectionAssert.Contains( columnTypes, ColumnType.Mentions );
			CollectionAssert.Contains( columnTypes, ColumnType.Messages );
			CollectionAssert.Contains( columnTypes, ColumnType.Timeline );
			CollectionAssert.Contains( columnTypes, ColumnType.Favorites );
			CollectionAssert.Contains( columnTypes, ColumnType.User );
		}

		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void TimelineColumnNavigatesToFinishPage()
		{
			// Arrange
			var wizard = new Mock<IWizardViewModel>();
			wizard.Setup( w => w.GotoPage( 3 ) ).Verifiable();
			wizard.Setup( w => w.GetProperty<ulong[]>( AddColumnDialogViewModel.SourceAccountsKey ) ).Returns( new ulong[] {123} );
			wizard.Setup( w => w.GetProperty<ulong[]>( AddColumnDialogViewModel.TargetAccountsKey ) ).Returns( new ulong[] {222} );
			wizard.Setup( w => w.SetProperty( AddColumnDialogViewModel.TargetAccountsKey, new ulong[] {222, 123} ) ).Verifiable();
			var page = new ColumnTypeSelctorPage( wizard.Object );

			// Act
			page.GotoNextPageCommand.Execute( ColumnType.Timeline );

			// Assert
			wizard.Verify( w => w.GotoPage( 3 ), Times.Once() );
			wizard.Verify( w => w.SetProperty( AddColumnDialogViewModel.TargetAccountsKey, new ulong[] {222, 123} ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void UserColumnNavigatesToUserPage()
		{
			// Arrange
			var wizard = new Mock<IWizardViewModel>();
			wizard.Setup( w => w.GotoPage( 2 ) ).Verifiable();
			var page = new ColumnTypeSelctorPage( wizard.Object );

			// Act
			page.GotoNextPageCommand.Execute( ColumnType.User );

			// Assert
			wizard.Verify( w => w.GotoPage( 2 ), Times.Once() );
		}
	}
}