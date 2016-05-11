using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Profile
{
	[TestClass]
	public class ProfileDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public async Task UserDataIsLoaded()
		{
			// Arrange
			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.DisplayMessage( Strings.UserNotFound, NotificationType.Error ) ).Verifiable();

			var user = DummyGenerator.CreateDummyUser();
			user.UserID = 123;

			var contextList = new Mock<ITwitterContextList>();
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 444 );
			context.Setup( c => c.Twitter.Users.ShowUser( 123ul, It.IsAny<bool>() ) ).Returns( Task.FromResult( user ) );
			context.Setup( c => c.Twitter.Friendships.GetFriendshipWith( 444, 123 ) ).Returns( Task.FromResult( new Friendship
			{
				TargetRelationship = new Relationship
				{
					FollowedBy = true,
					Following = true
				}
			} ) );

			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} ).Verifiable();
			var vm = new ProfileDialogViewModel
			{
				ContextList = contextList.Object,
				Notifier = notifier.Object,
				Dispatcher = new SyncDispatcher()
			};

			vm.Setup( 123 );

			// Act
			await vm.OnLoad( null );

			// Assert
			Assert.AreEqual( user.UserID, vm.User.UserId );
			Assert.IsTrue( vm.Friendship.TargetRelationship.Following );
			Assert.IsTrue( vm.Friendship.TargetRelationship.FollowedBy );
		}

		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var typeResolver = new Mock<ITypeResolver>();
			typeResolver.Setup( t => t.Resolve( typeof( UserViewModel ) ) ).Returns( new UserViewModel( DummyGenerator.CreateDummyUser() ) );

			var vm = new ProfileDialogViewModel();
			var tester = new PropertyChangedTester( vm, false, typeResolver.Object );

			// Act
			tester.Test( nameof( ProfileDialogViewModel.Notifier ) );

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Profie" )]
		public async Task ProfileForInvalidUserDoesNothing()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new IContextEntry[0] ).Verifiable();
			var vm = new ProfileDialogViewModel
			{
				ContextList = contextList.Object,
				Dispatcher = new SyncDispatcher()
			};

			// Act
			await vm.OnLoad( null );

			// Assert
			contextList.VerifyGet( c => c.Contexts, Times.Never() );
		}

		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public async Task ProfileForNonExistingUserRaisesError()
		{
			// Arrange
			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.DisplayMessage( Strings.UserNotFound, NotificationType.Error ) ).Verifiable();

			var contextList = new Mock<ITwitterContextList>();
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.Users.ShowUser( 123ul, It.IsAny<bool>() ) ).Returns( Task.FromResult<User>( null ) );

			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} ).Verifiable();
			var vm = new ProfileDialogViewModel
			{
				ContextList = contextList.Object,
				Notifier = notifier.Object,
				Dispatcher = new SyncDispatcher()
			};

			vm.Setup( 123 );

			// Act
			await vm.OnLoad( null );

			// Assert
			notifier.Verify( n => n.DisplayMessage( Strings.UserNotFound, NotificationType.Error ), Times.Once() );
		}
	}
}