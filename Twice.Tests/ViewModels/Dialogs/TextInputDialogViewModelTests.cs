using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.ViewModels.Dialogs;

namespace Twice.Tests.ViewModels.Dialogs
{
	[TestClass, ExcludeFromCodeCoverage]
	public class TextInputDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Dialogs" )]
		public void InputMustNotBeEmpty()
		{
			// Arrange
			var vm = new TextInputDialogViewModel
			{
				Input = "test"
			};
			vm.Input = null;

			// Act
			bool nul = vm.OkCommand.CanExecute( null );
			vm.Input = string.Empty;
			bool empty = vm.OkCommand.CanExecute( null );
			vm.Input = "test";
			bool text = vm.OkCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( nul );
			Assert.IsFalse( empty );
			Assert.IsTrue( text );
		}

		[TestMethod, TestCategory( "ViewModels.Dialogs" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var vm = new TextInputDialogViewModel();
			var tester = new PropertyChangedTester( vm );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}
	}
}