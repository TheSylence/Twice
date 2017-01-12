using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Twice.ViewModels.Wizards;

namespace Twice.Tests.ViewModels.Wizards
{
	[TestClass, ExcludeFromCodeCoverage]
	public class WizardViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Wizards" )]
		public void FinishCanAlwaysBeExecuted()
		{
			// Arrange
			var vm = CreateVm();

			// Act
			bool canExecute = vm.FinishCommand.CanExecute( null );

			// Assert
			Assert.IsTrue( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Wizards" )]
		public void FinishCommandClosesWizard()
		{
			// Arrange
			var vm = CreateVm();
			vm.Dispatcher = new SyncDispatcher();

			bool closeRequested = false;
			vm.CloseRequested += ( s, e ) => closeRequested = true;

			// Act
			vm.FinishCommand.Execute( null );

			// Assert
			Assert.IsTrue( closeRequested );
		}

		[TestMethod, TestCategory( "ViewModels.Wizards" )]
		public void PropertiesCanBeStored()
		{
			// Arrange
			var vm = CreateVm();

			// Act
			vm.SetProperty( "test", 123 );
			var prop = vm.GetProperty<int>( "test" );

			// Assert
			Assert.AreEqual( 123, prop );
		}

		[TestMethod, TestCategory( "ViewModels.Wizards" )]
		public void PropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var vm = CreateVm();
			var resolver = new Mock<ITypeResolver>();
			resolver.Setup( c => c.Resolve( typeof( WizardPageViewModel ) ) ).Returns( new Mock<WizardPageViewModel>( null ).Object );
			var tester = new PropertyChangedTester( vm, true, resolver.Object );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		private static WizardViewModel CreateVm()
		{
			return new Mock<WizardViewModel>
			{
				CallBase = true
			}.Object;
		}
	}
}