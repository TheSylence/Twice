using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class TweetDetailsViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public async Task LoadingWithoutTweetClosesDialog()
		{
			// Arrange
			var vm = new TweetDetailsViewModel
			{
				Dispatcher = new SyncDispatcher()
			};
			bool closed = false;
			vm.CloseRequested += ( s, e ) => closed = true;

			// Act
			await vm.OnLoad( null );

			// Assert
			Assert.IsTrue( closed );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var typeResolver = new Mock<ITypeResolver>();
			typeResolver.Setup( t => t.Resolve( typeof( StatusViewModel ) ) ).Returns( new StatusViewModel( DummyGenerator.CreateDummyStatus(), null, null, null ) );

			var vm = new TweetDetailsViewModel();
			var tester = new PropertyChangedTester( vm, false, typeResolver.Object );

			// Act
			tester.Test( nameof( TweetDetailsViewModel.Context ) );

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public async Task RetweetersAreLoaded()
		{
			// Arrange
			var status = DummyGenerator.CreateDummyStatus();
			status.ID = 123;

			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.Search.SearchReplies( It.IsAny<Status>() ) ).Returns( Task.FromResult( new List<Status>() ) );
			context.Setup( c => c.Twitter.Users.LookupUsers( It.IsAny<IEnumerable<ulong>>() ) ).Returns( Task.FromResult( new List<User>() ) );
			context.Setup( c => c.Twitter.Statuses.FindRetweeters( 123, It.IsAny<int>() ) ).Returns( Task.FromResult( new List<ulong>() ) ).Verifiable();

			var statusVm = new StatusViewModel( status, context.Object, null, null );
			var vm = new TweetDetailsViewModel
			{
				DisplayTweet = statusVm,
				Dispatcher = new SyncDispatcher(),
				Context = context.Object
			};

			// Act
			await vm.OnLoad( null );

			// Assert
			context.Verify( c => c.Twitter.Statuses.FindRetweeters( 123, It.IsAny<int>() ), Times.Once() );
		}
	}
}