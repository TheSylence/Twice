using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass]
	public class MuteEditViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void CancelRaisesEvent()
		{
			// Arrange
			var vm = new MuteEditViewModel( MuteEditAction.Add );
			bool raised = false;
			vm.Cancelled += ( s, e ) => raised = true;

			// Act
			vm.CancelCommand.Execute( null );

			// Assert
			Assert.IsTrue( raised );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void DefaultEndDateIsInTheFuture()
		{
			// Arrange
			var vm = new MuteEditViewModel( MuteEditAction.Add );

			// Act
			var end = vm.EndDate;

			// Assert
			Assert.IsTrue( end > DateTime.Now );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void InputValidationIsApplied()
		{
			// Arrange
			var vm = new MuteEditViewModel( MuteEditAction.Add );

			// Act
			vm.Filter = string.Empty;
			bool allEmpty = vm.SaveCommand.CanExecute( null );

			vm.Filter = "test";
			vm.HasEndDate = true;
			vm.EndDate = DateTime.Now.AddDays( -1 );
			bool withEndDateInPast = vm.SaveCommand.CanExecute( null );

			vm.HasEndDate = false;
			vm.Filter = string.Empty;
			bool withEmptyFilter = vm.SaveCommand.CanExecute( null );

			vm.Filter = "test";
			bool ok = vm.SaveCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( allEmpty );
			Assert.IsFalse( withEndDateInPast );
			Assert.IsFalse( withEmptyFilter );
			Assert.IsTrue( ok );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var vm = new MuteEditViewModel( MuteEditAction.Add );
			var tester = new PropertyChangedTester( vm );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SaveCommandRaisesEvent()
		{
			// Arrange
			var vm = new MuteEditViewModel( MuteEditAction.Add );
			MuteEditArgs args = null;
			vm.Saved += ( s, e ) => args = e;

			// Act
			vm.Filter = "test";
			vm.HasEndDate = true;
			vm.SaveCommand.Execute( null );

			// Assert
			Assert.IsNotNull( args );
			Assert.AreEqual( vm.Filter, args.Filter );
			Assert.AreEqual( vm.EndDate, args.EndDate );
			Assert.AreEqual( MuteEditAction.Add, args.Action );
		}
	}
}