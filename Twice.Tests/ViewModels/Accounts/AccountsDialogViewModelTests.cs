using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Utilities.Os;
using Twice.ViewModels.Accounts;

namespace Twice.Tests.ViewModels.Accounts
{
	[TestClass]
	public class AccountsDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Accounts" )]
		public void AccountListIsPopulated()
		{
			// Arrange
			var ctx1 = new Mock<IContextEntry>();
			ctx1.Setup( c => c.AccountName ).Returns( "1" );
			var ctx2 = new Mock<IContextEntry>();
			ctx2.Setup( c => c.AccountName ).Returns( "2" );
			var ctx3 = new Mock<IContextEntry>();
			ctx3.Setup( c => c.AccountName ).Returns( "3" );

			var columnList = new Mock<IColumnDefinitionList>();
			var contextList = new Mock<ITwitterContextList>();
			contextList.Setup( c => c.Contexts ).Returns( new[]
			{
				ctx1.Object,
				ctx2.Object,
				ctx3.Object,
			} );

			// Act
			var vm = new AccountsDialogViewModel( columnList.Object, contextList.Object, null );

			// Assert
			Assert.AreEqual( 3, vm.AddedAccounts.Count );
			Assert.IsNotNull( vm.AddedAccounts.SingleOrDefault( a => a.AccountName == "1" ) );
			Assert.IsNotNull( vm.AddedAccounts.SingleOrDefault( a => a.AccountName == "2" ) );
			Assert.IsNotNull( vm.AddedAccounts.SingleOrDefault( a => a.AccountName == "3" ) );
		}

		[TestMethod, TestCategory( "ViewModels.Accounts" )]
		public void PinPageIsDisplayed()
		{
			// Arrange
			var columnList = new Mock<IColumnDefinitionList>();
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new IContextEntry[0] );
			var auth = new Mock<ITwitterAuthorizer>();
			auth.Setup( a => a.Authorize( It.IsAny<Action<string>>(), It.IsAny<Func<string>>(), It.IsAny<CancellationToken>() ) )
				.Callback<Action<string>, Func<string>, CancellationToken?>( ( display, getter, token ) => display( "test" ) )
				.Returns( Task.FromResult( new AuthorizeResult( null, null ) ) );
			var vm = new AccountsDialogViewModel( columnList.Object, contextList.Object, auth.Object );

			var procStarter = new Mock<IProcessStarter>();
			procStarter.Setup( p => p.Start( It.IsAny<string>() ) )
				.Verifiable();
			vm.ProcessStarter = procStarter.Object;

			// Act
			vm.AddAccountCommand.Execute( null );

			// Assert
			procStarter.Verify( p => p.Start( "test" ), Times.Once() );
		}
	}
}