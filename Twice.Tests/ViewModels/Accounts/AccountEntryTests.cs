using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Twice.Models.Twitter;
using Twice.ViewModels.Accounts;

namespace Twice.Tests.ViewModels.Accounts
{
	[TestClass]
	public class AccountEntryTests
	{
		[TestMethod, TestCategory( "ViewModels.Accounts" )]
		public void ConstrutionIsCorrect()
		{
			// Arrange
			var contextMock = new Mock<IContextEntry>();
			contextMock.SetupGet( c => c.AccountName ).Returns( "testi" );
			contextMock.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com/image.png" ) );
			contextMock.SetupGet( c => c.RequiresConfirmation ).Returns( true );
			contextMock.SetupGet( c => c.IsDefault ).Returns( false );

			var context = contextMock.Object;

			// Act
			var entry = new AccountEntry( context );

			// Assert
			Assert.AreEqual( context.AccountName, entry.AccountName );
			Assert.AreEqual( context.IsDefault, entry.IsDefaultAccount );
			Assert.AreEqual( context.RequiresConfirmation, entry.RequiresConfirmation );
			Assert.AreEqual( context.ProfileImageUrl, entry.ProfileImage );
		}

		[TestMethod, TestCategory( "ViewModels.Accounts" )]
		public void SettingConfirmationUpdatesContext()
		{
			// Arrange
			var data = new TwitterAccountData
			{
				RequiresConfirm = false
			};

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.Data ).Returns( data );

			// Act
			var entry = new AccountEntry( context.Object )
			{
				RequiresConfirmation = true
			};

			// Assert
			Assert.IsNotNull( entry );
			Assert.IsTrue( data.RequiresConfirm );
		}

		[TestMethod, TestCategory( "ViewModels.Accounts" )]
		public void SettingDefaultUpdatesContext()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupSet( c => c.IsDefault = true ).Verifiable();

			// Act
			var entry = new AccountEntry( context.Object )
			{
				IsDefaultAccount = true
			};

			// Assert
			Assert.IsNotNull( entry );
			context.VerifySet( c => c.IsDefault = true, Times.Once() );
		}
	}
}