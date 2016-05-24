using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Twitter;
using Twice.ViewModels.Wizards;

namespace Twice.Tests.ViewModels.ColumnManagement
{
	[TestClass, ExcludeFromCodeCoverage]
	public class UserSelectorPageTests
	{
		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void GotoNextPageFillsWizardWithCorrectData()
		{
			// Arrange
			var wizard = new Mock<IWizardViewModel>();
			wizard.Setup( w => w.GetProperty<ulong[]>( AddColumnDialogViewModel.TargetAccountsKey ) ).Returns( new ulong[0] );
			wizard.Setup( w => w.SetProperty( AddColumnDialogViewModel.TargetAccountsKey, new ulong[] {123} ) ).Verifiable();
			wizard.Setup( w => w.GetProperty<string[]>( AddColumnDialogViewModel.TargetAccountNamesKey ) ).Returns( new string[0] );
			wizard.Setup( w => w.SetProperty( AddColumnDialogViewModel.TargetAccountNamesKey, new[] {"Dummy"} ) ).Verifiable();
			wizard.Setup( w => w.GotoPage( 3 ) ).Verifiable();

			var timerFactory = new Mock<ITimerFactory>();
			var timer = new Mock<ITimer>();
			timerFactory.Setup( f => f.Create( It.IsAny<int>() ) ).Returns( timer.Object );

			var vm = new UserSelectorPage( wizard.Object, timerFactory.Object );
			var dummyUser = DummyGenerator.CreateDummyUser();
			dummyUser.ScreenName = "Dummy";
			dummyUser.UserID = 123;
			vm.Users.Add( new UserViewModel( dummyUser ) );

			// Act
			vm.GotoNextPageCommand.Execute( 123ul );

			// Assert
			wizard.Verify( w => w.SetProperty( AddColumnDialogViewModel.TargetAccountsKey, new ulong[] {123} ), Times.Once() );
			wizard.Verify( w => w.SetProperty( AddColumnDialogViewModel.TargetAccountNamesKey, new[] {"Dummy"} ), Times.Once() );
			wizard.Verify( w => w.GotoPage( 3 ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.ColumnManagement" )]
		public void TextInputStartsSearchTimer()
		{
			// Arrange
			var wizard = new Mock<IWizardViewModel>();
			var timerFactory = new Mock<ITimerFactory>();
			var timer = new Mock<ITimer>();
			timer.Setup( t => t.Start() ).Verifiable();
			timer.Setup( t => t.Stop() ).Verifiable();
			timerFactory.Setup( f => f.Create( 1000 ) ).Returns( timer.Object );

			var vm = new UserSelectorPage( wizard.Object, timerFactory.Object )
			{
				SearchText = null
			};

			// Act
			vm.SearchText = "test";

			// Assert
			timer.Verify( t => t.Stop(), Times.Once() );
			timer.Verify( t => t.Start(), Times.Once() );
		}
	}
}