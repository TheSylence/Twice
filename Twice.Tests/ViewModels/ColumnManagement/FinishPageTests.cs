using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Wizards;

namespace Twice.Tests.ViewModels.ColumnManagement
{
	[TestClass, ExcludeFromCodeCoverage]
	public class FinishPageTests
	{
		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void FinishPageIsLastPage()
		{
			// Arrange
			var wizard = new Mock<IWizardViewModel>();
			var page = new FinishPage( wizard.Object );

			// Act
			bool last = page.IsLastPage;

			// Assert
			Assert.IsTrue( last );
		}

		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var wizard = new Mock<IWizardViewModel>();
			var obj = new FinishPage( wizard.Object );
			var tester = new PropertyChangedTester( obj );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void SourceAccountsAreLoadedCorrectly()
		{
			// Arrange
			var wizard = new Mock<IWizardViewModel>();
			wizard.Setup( w => w.GetProperty<string[]>( AddColumnDialogViewModel.SourceAccountNamesKey ) ).Returns( new[]
			{"Acc1", "Acc2"} );

			var page = new FinishPage( wizard.Object );

			// Act
			page.OnNavigatedTo( true );

			// Assert
			Assert.AreEqual( "Acc1, Acc2", page.SourceAccount );
		}
	}
}