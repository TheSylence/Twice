using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.Utilities.Os;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass]
	public class StatusViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void CopyTweetUrlWritesToClipboard()
		{
			// Arrange
			var context = new Mock<IContextEntry>();

			var status = DummyGenerator.CreateDummyStatus();
			status.Text = "hello world";
			status.User.ScreenName = "Testi";
			status.ID = 123;
			var vm = new StatusViewModel( status, context.Object );

			var clipboard = new Mock<IClipboard>();
			clipboard.Setup( c => c.SetText( It.Is<string>( str => Uri.IsWellFormedUriString( str, UriKind.Absolute ) ) ) ).Verifiable();
			vm.Clipboard = clipboard.Object;

			// Act
			vm.CopyTweetUrlCommand.Execute( null );

			// Assert
			clipboard.VerifyAll();
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void CopyTweetWritesToClipboard()
		{
			// Arrange
			var context = new Mock<IContextEntry>();

			var status = DummyGenerator.CreateDummyStatus();
			status.Text = "hello world";
			status.User.ScreenName = "Testi";
			var vm = new StatusViewModel( status, context.Object );

			var clipboard = new Mock<IClipboard>();
			clipboard.Setup( c => c.SetText( "@Testi: hello world" ) ).Verifiable();
			vm.Clipboard = clipboard.Object;

			// Act
			vm.CopyTweetCommand.Execute( null );

			// Assert
			clipboard.Verify( c => c.SetText( "@Testi: hello world" ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ForeignStatusCanBeReportedAsSpam()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 222;
			var vm = new StatusViewModel( status, context.Object );

			// Act
			bool canExecute = vm.ReportSpamCommand.CanExecute( null );

			// Assert
			Assert.IsTrue( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ForeignStatusCanBeRetweeted()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 222;
			var vm = new StatusViewModel( status, context.Object );

			// Act
			bool canExecute = vm.RetweetStatusCommand.CanExecute( null );

			// Assert
			Assert.IsTrue( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ForeignStatusCannotBeDeleted()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 222;
			var vm = new StatusViewModel( status, context.Object );

			// Act
			bool canExecute = vm.DeleteStatusCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ForeignUserCanBeBlocked()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 222;
			var vm = new StatusViewModel( status, context.Object );

			// Act
			bool canExecute = vm.BlockUserCommand.CanExecute( null );

			// Assert
			Assert.IsTrue( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void OwnStatusCanBeDeleted()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 123;
			var vm = new StatusViewModel( status, context.Object );

			// Act
			bool canExecute = vm.DeleteStatusCommand.CanExecute( null );

			// Assert
			Assert.IsTrue( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void OwnStatusCannotBeReportedAsSpam()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 123;
			var vm = new StatusViewModel( status, context.Object );

			// Act
			bool canExecute = vm.ReportSpamCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void OwnStatusCannotBeRetweeted()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 123;
			var vm = new StatusViewModel( status, context.Object );

			// Act
			bool canExecute = vm.RetweetStatusCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void OwnUserCannotBeBlocked()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 123;
			var vm = new StatusViewModel( status, context.Object );

			// Act
			bool canExecute = vm.BlockUserCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ReplyToAllNeedsAtLeastTwoUsers()
		{
			// Arrange
			var context = new Mock<IContextEntry>();

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 123;
			status.Entities = new LinqToTwitter.Entities
			{
				UserMentionEntities = new System.Collections.Generic.List<LinqToTwitter.UserMentionEntity>
				{
					new LinqToTwitter.UserMentionEntity {Id = 123}
				}
			};
			var vm = new StatusViewModel( status, context.Object );

			// Act
			bool single = vm.ReplyToAllCommand.CanExecute( null );
			status.User.UserID = 222;
			bool multiple = vm.ReplyToAllCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( single );
			Assert.IsTrue( multiple );
		}
	}
}