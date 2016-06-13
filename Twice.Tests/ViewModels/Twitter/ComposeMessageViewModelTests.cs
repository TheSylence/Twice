using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ComposeMessageViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void EnteringRecipientQueriesFriendship()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );
			context.Setup( c => c.Twitter.Friendships.GetFriendshipWith( 123, "the_username" ) ).Returns( Task.FromResult( new LinqToTwitter.Friendship() ) );

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} );

			var vm = new ComposeMessageViewModel
			{
				ContextList = contextList.Object
			};

			var waitHandle = new ManualResetEventSlim( false );
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( ComposeMessageViewModel.IsCheckingRelationship ) && vm.IsCheckingRelationship == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.Recipient = "the_username";

			bool set = waitHandle.Wait( 1000 );

			// Assert
			Assert.IsTrue( set );
			context.Verify( c => c.Twitter.Friendships.GetFriendshipWith( 123, "the_username" ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void MessageNeedsTextAndRecipient()
		{
			// Arrange
			var vm = new ComposeMessageViewModel
			{
				Recipient = string.Empty
			};

			// Act
			bool noData = vm.OkCommand.CanExecute( null );

			vm.Recipient = "test";
			vm.Text = string.Empty;
			bool noText = vm.OkCommand.CanExecute( null );

			vm.Recipient = string.Empty;
			vm.Text = "test";
			bool noRecipient = vm.OkCommand.CanExecute( null );

			vm.Recipient = "test";
			bool allData = vm.OkCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( noData );
			Assert.IsFalse( noText );
			Assert.IsFalse( noRecipient );
			Assert.IsTrue( allData );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var resolver = new Mock<ITypeResolver>();
			var context = new Mock<IContextEntry>();
			resolver.Setup( r => r.Resolve( typeof( MessageViewModel ) ) )
				.Returns( new MessageViewModel( DummyGenerator.CreateDummyMessage(), context.Object, null ) );

			var vm = new ComposeMessageViewModel();
			var tester = new PropertyChangedTester( vm, false, resolver.Object );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void SendCommandCallsTwitterApi()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.SendMessage( "the_user", "the_message" ) ).Returns( Task.FromResult( new LinqToTwitter.DirectMessage() ) );

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} );

			var vm = new ComposeMessageViewModel
			{
				ContextList = contextList.Object
			};

			var waitHandle = new ManualResetEventSlim( false );
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( ComposeMessageViewModel.IsSending ) && vm.IsSending == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.Recipient = "the_user";
			vm.Text = "the_message";
			vm.OkCommand.Execute( null );
			bool set = waitHandle.Wait( 1000 );

			// Assert
			Assert.IsTrue( set );
			context.Verify( c => c.Twitter.SendMessage( "the_user", "the_message" ), Times.Once() );
		}
	}
}