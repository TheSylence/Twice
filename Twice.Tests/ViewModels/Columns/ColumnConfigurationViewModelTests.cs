using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Models.Columns;
using Twice.ViewModels.Columns;

namespace Twice.Tests.ViewModels.Columns
{
	[TestClass]
	public class ColumnConfigurationViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void PropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var vm = new ColumnConfigurationViewModel( new ColumnDefinition( ColumnType.Activity ) );
			var tester = new PropertyChangedTester( vm );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void SaveCommandRaisesEventWhenChanged()
		{
			// Arrange
			var vm = new ColumnConfigurationViewModel( new ColumnDefinition( ColumnType.Activity ) );
			bool raised = false;
			vm.Saved += ( s, e ) => raised = true;

			// Act
			vm.SaveCommand.Execute( null );
			bool noChange = raised;

			vm.PopupEnabled = !vm.PopupEnabled;
			vm.SaveCommand.Execute( null );
			bool change = raised;

			// Assert
			Assert.IsFalse( noChange );
			Assert.IsTrue( change );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void ValuesAreResettedOnExpand()
		{
			// Arrange
			var def = new ColumnDefinition( ColumnType.Activity )
			{
				Notifications =
				{
					Popup = true,
					Sound = true,
					Toast = true
				}
			};
			var vm = new ColumnConfigurationViewModel( def );

			// Act
			var popupBefore = vm.PopupEnabled;
			var soundBefore = vm.SoundEnabled;
			var toastBefore = vm.ToastsEnabled;

			vm.IsExpanded = true;

			var popupAfter = vm.PopupEnabled;
			var soundAfter = vm.SoundEnabled;
			var toastAfter = vm.ToastsEnabled;

			// Assert
			Assert.IsFalse( popupBefore );
			Assert.IsFalse( soundBefore );
			Assert.IsFalse( toastBefore );

			Assert.IsTrue( popupAfter );
			Assert.IsTrue( soundAfter );
			Assert.IsTrue( toastAfter );
		}
	}
}