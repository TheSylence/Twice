using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Entities;
using Twice.Resources;
using Twice.ViewModels;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Profile
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ProfileDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public void ExceptionDuringFollowIsCaught()
		{
			// Arrange
			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.DisplayMessage( "Test", NotificationType.Error ) ).Verifiable();

			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.Users.FollowUser( 123 ) ).Throws( new TwitterQueryException( "Test" ) );

			var vm = new ProfileDialogViewModel
			{
				Notifier = notifier.Object,
				User = new UserViewModel( DummyGenerator.CreateDummyUser( 123 ) )
			};
			vm.OverwriteContext( context.Object );

			// Act
			vm.FollowUserCommand.Execute( null );

			// Assert
			notifier.Verify( n => n.DisplayMessage( "Test", NotificationType.Error ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public void ExceptionDuringUnfollowIsCaught()
		{
			// Arrange
			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.DisplayMessage( "Test", NotificationType.Error ) ).Verifiable();

			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.Users.UnfollowUser( 123 ) ).Throws( new TwitterQueryException( "Test" ) );

			var vm = new ProfileDialogViewModel
			{
				Notifier = notifier.Object,
				User = new UserViewModel( DummyGenerator.CreateDummyUser( 123 ) )
			};
			vm.OverwriteContext( context.Object );

			// Act
			vm.UnfollowUserCommand.Execute( null );

			// Assert
			notifier.Verify( n => n.DisplayMessage( "Test", NotificationType.Error ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public async Task FollowersAreLoaded()
		{
			// Arrange
			var waitHandle = new ManualResetEventSlim( false );

			var user = DummyGenerator.CreateDummyUserEx();
			user.UserID = 123;

			var contextList = new Mock<ITwitterContextList>();
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 444 );
			context.Setup( c => c.Twitter.Users.ShowUser( 123ul, It.IsAny<bool>() ) ).Returns( Task.FromResult( user ) );
			context.Setup( c => c.Twitter.Friendships.ListFollowers( 123, 200, true ) ).Returns(
				Task.FromResult( new List<User> {user} ) );
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
				Dispatcher = new SyncDispatcher()
			};

			vm.Setup( 123 );
			await vm.OnLoad( null );

			var page = vm.UserPages.Single( p => p.Title == Strings.Followers );

			page.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( UserSubPage.IsLoading ) && page.IsLoading == false )
				{
					Thread.Sleep( 50 );
					waitHandle.Set();
				}
			};

			// Act
			var ignore = page.Items;
			waitHandle.Wait( 1000 );
			var items = page.Items;

			// Assert
			Assert.IsNotNull( items.SingleOrDefault() );

			context.Verify( c => c.Twitter.Friendships.ListFollowers( 123, 200, true ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public async Task FriendsAreLoaded()
		{
			// Arrange
			var waitHandle = new ManualResetEventSlim( false );

			var user = DummyGenerator.CreateDummyUserEx();
			user.UserID = 123;

			var contextList = new Mock<ITwitterContextList>();
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 444 );
			context.Setup( c => c.Twitter.Users.ShowUser( 123ul, It.IsAny<bool>() ) ).Returns( Task.FromResult( user ) );
			context.Setup( c => c.Twitter.Friendships.ListFriends( 123, 200, true ) ).Returns(
				Task.FromResult( new List<User> {user} ) );
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
				Dispatcher = new SyncDispatcher()
			};

			vm.Setup( 123 );
			await vm.OnLoad( null );

			var page = vm.UserPages.Single( p => p.Title == Strings.Following );

			page.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( UserSubPage.IsLoading ) && page.IsLoading == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			var nullItems = page.Items;

			waitHandle.Wait( 1000 );

			var items = page.Items;

			// Assert
			Assert.IsNull( nullItems );
			Assert.IsNotNull( items.SingleOrDefault() );

			context.Verify( c => c.Twitter.Friendships.ListFriends( 123, 200, true ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public async Task MoreStatusesCanBeLoaded()
		{
			// Arrange
			var waitHandle = new ManualResetEventSlim( false );

			var user = DummyGenerator.CreateDummyUserEx();
			user.UserID = 123;

			var firstStatus = DummyGenerator.CreateDummyStatus( user );
			firstStatus.ID = firstStatus.StatusID = 456;

			var secondStatus = DummyGenerator.CreateDummyStatus( user );
			secondStatus.ID = secondStatus.StatusID = 555;

			var contextList = new Mock<ITwitterContextList>();
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 444 );
			context.Setup( c => c.Twitter.Users.ShowUser( 123ul, It.IsAny<bool>() ) ).Returns( Task.FromResult( user ) );
			context.Setup( c => c.Twitter.Statuses.GetUserTweets( 123, 0, 0 ) ).Returns(
				Task.FromResult( new List<Status> {firstStatus} ) ).Verifiable();
			context.Setup( c => c.Twitter.Statuses.GetUserTweets( 123, 0, 456 ) ).Returns(
				Task.FromResult( new List<Status> {secondStatus} ) ).Verifiable();
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
				Dispatcher = new SyncDispatcher()
			};

			vm.Setup( 123 );
			await vm.OnLoad( null );

			var page = vm.UserPages.Single( p => p.Title == Strings.Tweets );

			page.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( UserSubPage.IsLoading ) && page.IsLoading == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			var nullItems = page.Items;

			waitHandle.Wait( 1000 );

			waitHandle.Reset();
			page.ActionDispatcher.OnBottomReached();
			waitHandle.Wait( 1000 );

			var items = page.Items;

			// Assert
			Assert.IsNull( nullItems );
			Assert.AreEqual( 2, items.Count );

			context.Verify( c => c.Twitter.Statuses.GetUserTweets( 123, 0, 456 ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var typeResolver = new Mock<ITypeResolver>();
			typeResolver.Setup( t => t.Resolve( typeof(UserViewModel) ) ).Returns(
				new UserViewModel( DummyGenerator.CreateDummyUserEx() ) );

			var vm = new ProfileDialogViewModel();
			var tester = new PropertyChangedTester( vm, false, typeResolver.Object );

			// Act
			tester.Test();

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
			context.Setup( c => c.Twitter.Users.ShowUser( 123ul, It.IsAny<bool>() ) ).Returns( Task.FromResult<UserEx>( null ) );

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

		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public async Task UserDataIsLoaded()
		{
			// Arrange
			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.DisplayMessage( Strings.UserNotFound, NotificationType.Error ) ).Verifiable();

			var user = DummyGenerator.CreateDummyUserEx();
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
		public async Task UsersStatusesAreLoaded()
		{
			// Arrange
			var waitHandle = new ManualResetEventSlim( false );

			var user = DummyGenerator.CreateDummyUserEx();
			user.UserID = 123;

			var status = DummyGenerator.CreateDummyStatus( user );
			status.ID = 456;

			var contextList = new Mock<ITwitterContextList>();
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 444 );
			context.Setup( c => c.Twitter.Users.ShowUser( 123ul, It.IsAny<bool>() ) ).Returns( Task.FromResult( user ) );
			context.Setup( c => c.Twitter.Statuses.GetUserTweets( 123, 0, 0 ) ).Returns(
				Task.FromResult( new List<Status> {status} ) ).Verifiable();
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
				Dispatcher = new SyncDispatcher()
			};

			vm.Setup( 123 );
			await vm.OnLoad( null );

			var page = vm.UserPages.Single( p => p.Title == Strings.Tweets );

			page.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( UserSubPage.IsLoading ) && page.IsLoading == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			var nullItems = page.Items;

			waitHandle.Wait( 1000 );

			var items = page.Items;

			// Assert
			Assert.IsNull( nullItems );
			Assert.IsNotNull( items.SingleOrDefault() );

			context.Verify( c => c.Twitter.Statuses.GetUserTweets( 123, 0, 0 ), Times.Once() );
		}
	}
}