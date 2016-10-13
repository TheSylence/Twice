using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Cache;
using Twice.Models.Configuration;
using Twice.Models.Scheduling;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels;
using Twice.ViewModels.Twitter;
using Twice.Views.Services;
using MediaType = LinqToTwitter.MediaType;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ComposeTweetViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void AttachImageUploadsToTwitter()
		{
			// Arrange
			var waitHandle = new ManualResetEventSlim( false );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.OpenFile( It.IsAny<FileServiceArgs>() ) ).Returns( Task.FromResult( "Data/Image.png" ) )
				.Verifiable();

			var twitterConfig = new Mock<ITwitterConfiguration>();
			twitterConfig.SetupGet( t => t.MaxImageSize ).Returns( int.MaxValue );

			var vm = new ComposeTweetViewModel
			{
				ViewServiceRepository = viewServices.Object,
				TwitterConfig = twitterConfig.Object
			};

			var media = new Media {MediaID = 123456, Type = MediaType.Image};

			const string mimeType = "image/png";

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com/file.name" ) );
			context.Setup( c => c.Twitter.UploadMediaAsync( It.IsAny<byte[]>(), mimeType, new ulong[0] ) ).Returns(
				Task.FromResult( media ) ).Verifiable();

			vm.Accounts.Add( new AccountEntry( context.Object, false ) {Use = true} );
			vm.Dispatcher = new SyncDispatcher();
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( ComposeTweetViewModel.IsSending ) && vm.IsSending == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.AttachImageCommand.Execute( null );
			waitHandle.Wait( 1000 );
			Thread.Sleep( 50 );

			// Assert
			context.Verify( c => c.Twitter.UploadMediaAsync( It.IsAny<byte[]>(), mimeType, new ulong[0] ), Times.Once() );

			Assert.IsNotNull( vm.AttachedMedias.SingleOrDefault( m => m.MediaId == media.MediaID ) );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void AttachingTooLargeImageRaisesError()
		{
			// Arrange
			var waitHandle = new ManualResetEventSlim( false );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.OpenFile( It.IsAny<FileServiceArgs>() ) ).Returns( Task.FromResult( "Data/Image.png" ) )
				.Verifiable();

			var twitterConfig = new Mock<ITwitterConfiguration>();
			twitterConfig.SetupGet( t => t.MaxImageSize ).Returns( 1 );

			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.DisplayMessage( Strings.ImageSizeTooBig, NotificationType.Error ) ).Verifiable();

			var vm = new ComposeTweetViewModel
			{
				ViewServiceRepository = viewServices.Object,
				TwitterConfig = twitterConfig.Object,
				Notifier = notifier.Object
			};

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com/file.name" ) );
			vm.Accounts.Add( new AccountEntry( context.Object, false ) {Use = true} );

			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( ComposeTweetViewModel.IsSending ) && vm.IsSending == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.AttachImageCommand.Execute( null );
			waitHandle.Wait( 1000 );
			Thread.Sleep( 50 );

			// Assert
			notifier.Verify( n => n.DisplayMessage( Strings.ImageSizeTooBig, NotificationType.Error ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void CancellingImageSelectionDoesNotUpload()
		{
			// Arrange
			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.OpenFile( It.IsAny<FileServiceArgs>() ) ).Returns( Task.FromResult<string>( null ) )
				.Verifiable();

			var vm = new ComposeTweetViewModel
			{
				ViewServiceRepository = viewServices.Object
			};

			// Act
			vm.AttachImageCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.OpenFile( It.IsAny<FileServiceArgs>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public async Task DefaultAccountIsPreselectedForTweeting()
		{
			// Arrange
			var defAcc = new Mock<IContextEntry>();
			defAcc.SetupGet( a => a.IsDefault ).Returns( true );
			defAcc.SetupGet( a => a.ProfileImageUrl ).Returns( new Uri( "http://example.com" ) );
			var otherAcc = new Mock<IContextEntry>();
			otherAcc.SetupGet( a => a.IsDefault ).Returns( false );
			otherAcc.SetupGet( a => a.ProfileImageUrl ).Returns( new Uri( "http://example.com" ) );

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {otherAcc.Object, defAcc.Object} );

			var cache = new Mock<ICache>();

			var vm = new ComposeTweetViewModel
			{
				TwitterConfig = MockTwitterConfig(),
				ContextList = contextList.Object,
				Cache = cache.Object
			};

			// Act
			await vm.OnLoad( false );

			// Assert
			var usedAccount = vm.Accounts.SingleOrDefault( a => a.Use );
			Assert.IsNotNull( usedAccount );

			var notUsedAccount = vm.Accounts.SingleOrDefault( a => !a.Use );
			Assert.IsNotNull( notUsedAccount );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void DeletionDateMustBeInFutureWhenEnabled()
		{
			// Arrange
			var vm = new ComposeTweetViewModel();

			var pastDate = DateTime.Now.AddDays( -1 );
			var futureDate = DateTime.Now.AddDays( 1 );

			// Act
			vm.IsDeletionScheduled = false;
			vm.DeletionDate = pastDate;
			bool disabledDateInPast = vm.GetErrors( nameof( ComposeTweetViewModel.DeletionDate ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = false;
			vm.DeletionDate = futureDate;
			bool disabledDateInFuture = vm.GetErrors( nameof( ComposeTweetViewModel.DeletionDate ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = true;
			vm.DeletionDate = pastDate;
			bool enabledDateInPast = vm.GetErrors( nameof( ComposeTweetViewModel.DeletionDate ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = true;
			vm.DeletionDate = futureDate;
			bool enabledDateInFuture = vm.GetErrors( nameof( ComposeTweetViewModel.DeletionDate ) ).Cast<object>().Any();

			// Assert
			Assert.IsFalse( disabledDateInPast );
			Assert.IsFalse( disabledDateInFuture );
			Assert.IsTrue( enabledDateInPast );
			Assert.IsFalse( enabledDateInFuture );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void DeletionTimeMustBeInFutureWhenEnabled()
		{
			// Arrange
			var vm = new ComposeTweetViewModel();

			var pastDate = DateTime.Now.AddHours( -1 );
			var futureDate = DateTime.Now.AddHours( 1 );

			// Act
			vm.IsDeletionScheduled = false;
			vm.DeletionTime = pastDate;
			bool disabledDateInPast = vm.GetErrors( nameof( ComposeTweetViewModel.DeletionTime ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = false;
			vm.DeletionTime = futureDate;
			bool disabledDateInFuture = vm.GetErrors( nameof( ComposeTweetViewModel.DeletionTime ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = true;
			vm.DeletionTime = pastDate;
			bool enabledDateInPast = vm.GetErrors( nameof( ComposeTweetViewModel.DeletionTime ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = true;
			vm.DeletionTime = futureDate;
			bool enabledDateInFuture = vm.GetErrors( nameof( ComposeTweetViewModel.DeletionTime ) ).Cast<object>().Any();

			// Assert
			Assert.IsFalse( disabledDateInPast );
			Assert.IsFalse( disabledDateInFuture );
			Assert.IsTrue( enabledDateInPast );
			Assert.IsFalse( enabledDateInFuture );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ExceptionOnSendIsDisplayedInNotifier()
		{
			// Arrange
			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.DisplayMessage( "exception_message", NotificationType.Error ) ).Verifiable();

			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.Statuses.TweetAsync( It.IsAny<string>(), It.IsAny<IEnumerable<ulong>>(), 0 ) ).Throws(
				new Exception( "exception_message" ) );
			context.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com/file.name" ) );

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} );

			var vm = new ComposeTweetViewModel
			{
				Notifier = notifier.Object,
				ContextList = contextList.Object,
				TwitterConfig = new Mock<ITwitterConfiguration>().Object,
				Text = "the_text"
			};

			vm.Accounts.Add( new AccountEntry( context.Object, false ) {Use = true} );

			var waitHandle = new ManualResetEventSlim( false );
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( ComposeTweetViewModel.IsSending ) && vm.IsSending == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.SendTweetCommand.Execute( null );
			bool set = waitHandle.Wait( 1000 );

			// Assert
			Assert.IsTrue( set );
			notifier.Verify( n => n.DisplayMessage( "exception_message", NotificationType.Error ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void MediaCanBeRemoved()
		{
			// Arrange
			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.Confirm( It.IsAny<ConfirmServiceArgs>() ) ).Returns( Task.FromResult( true ) );

			var vm = new ComposeTweetViewModel();
			vm.AttachedMedias.Add( new MediaItem( 123, new byte[] {}, "test.png" ) );
			vm.ViewServiceRepository = viewServices.Object;
			vm.Dispatcher = new SyncDispatcher();

			// Act
			vm.DeleteMediaCommand.Execute( 111ul );
			bool wrongId = vm.AttachedMedias.Any( m => m.MediaId == 123 );
			vm.DeleteMediaCommand.Execute( 123ul );
			bool correctId = vm.AttachedMedias.Any( m => m.MediaId == 123 );

			// Assert
			Assert.IsTrue( wrongId );
			Assert.IsFalse( correctId );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public async Task OwnUserNameIsNotIncludedInReplyToAll()
		{
			// Arrange
			var user = DummyGenerator.CreateDummyUser();
			user.ScreenName = "you";
			var status = DummyGenerator.CreateDummyStatus( user );
			status.Entities.UserMentionEntities.Add( new UserMentionEntity {ScreenName = "me"} );
			status.Entities.UserMentionEntities.Add( new UserMentionEntity {ScreenName = "them"} );

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.AccountName ).Returns( "me" );
			context.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com/file.name" ) );

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} );

			var reply = new StatusViewModel( status, context.Object, null, null );

			var vm = new ComposeTweetViewModel
			{
				ContextList = contextList.Object,
				Dispatcher = new SyncDispatcher(),
				TwitterConfig = new Mock<ITwitterConfiguration>().Object,
				Cache = new Mock<ICache>().Object
			};
			vm.SetReply( reply, true );

			// Act
			await vm.OnLoad( false );

			// Assert
			Assert.IsTrue( vm.Text.Contains( "@them" ) );
			Assert.IsTrue( vm.Text.Contains( "@you" ) );
			Assert.IsFalse( vm.Text.Contains( "@me" ) );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void PropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var status = DummyGenerator.CreateDummyStatus();
			var typeResolver = new Mock<ITypeResolver>();
			typeResolver.Setup( t => t.Resolve( typeof(StatusViewModel) ) ).Returns( new StatusViewModel( status, null, null,
				null ) );

			var obj = new ComposeTweetViewModel
			{
				TwitterConfig = MockTwitterConfig()
			};
			var tester = new PropertyChangedTester( obj, false, typeResolver.Object );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void QuoteCanBeRemoved()
		{
			// Arrange
			var vm = new ComposeTweetViewModel();

			// Act
			bool without = vm.RemoveQuoteCommand.CanExecute( null );
			vm.QuotedTweet = new StatusViewModel( DummyGenerator.CreateDummyStatus(), null, null, null );
			bool with = vm.RemoveQuoteCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( without );
			Assert.IsTrue( with );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void QuotedTweetsUrlIsAppendedToUrl()
		{
			// Arrange
			var config = new Mock<IConfig>();
			var viewServiceRepo = new Mock<IViewServiceRepository>();

			var quotedTweet = DummyGenerator.CreateDummyStatus();
			var url = quotedTweet.GetUrl();

			var context = new Mock<IContextEntry>();
			var status = DummyGenerator.CreateDummyStatus();
			context.Setup( c => c.Twitter.Statuses.TweetAsync( "Hello world " + url, It.IsAny<IEnumerable<ulong>>(), 0 ) )
				.Returns(
					Task.FromResult( status ) ).Verifiable();
			context.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com/image.png" ) );

			var waitHandle = new ManualResetEventSlim( false );
			var vm = new ComposeTweetViewModel
			{
				TwitterConfig = MockTwitterConfig(),
				Text = "Hello world",
				QuotedTweet = new StatusViewModel( quotedTweet, context.Object, config.Object, viewServiceRepo.Object )
			};

			vm.Accounts.Add( new AccountEntry( context.Object, false ) {Use = true} );
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( ComposeTweetViewModel.IsSending ) && vm.IsSending == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.SendTweetCommand.Execute( null );
			waitHandle.Wait( 1000 );

			// Assert
			context.Verify( c => c.Twitter.Statuses.TweetAsync( "Hello world " + url, It.IsAny<IEnumerable<ulong>>(), 0 ),
				Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void RemoveQuoteRemoves()
		{
			// Arrange
			var vm = new ComposeTweetViewModel
			{
				QuotedTweet = new StatusViewModel( DummyGenerator.CreateDummyStatus(), null, null, null )
			};

			// Act
			vm.RemoveQuoteCommand.Execute( null );

			// Assert
			Assert.IsNull( vm.QuotedTweet );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void RemoveReplyCommandRemoves()
		{
			// Arrange
			var vm = new ComposeTweetViewModel
			{
				TwitterConfig = new Mock<ITwitterConfiguration>().Object,
				InReplyTo = new StatusViewModel( DummyGenerator.CreateDummyStatus(), null, null, null )
			};

			// Act
			vm.RemoveReplyCommand.Execute( null );

			// Assert
			Assert.IsNull( vm.InReplyTo );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ScheduleDateMustBeInFutureWhenEnabled()
		{
			// Arrange
			var vm = new ComposeTweetViewModel();

			var pastDate = DateTime.Now.AddDays( -1 );
			var futureDate = DateTime.Now.AddDays( 1 );

			// Act
			vm.IsTweetScheduled = false;
			vm.ScheduleDate = pastDate;
			bool disabledDateInPast = vm.GetErrors( nameof( ComposeTweetViewModel.ScheduleDate ) ).Cast<object>().Any();

			vm.IsTweetScheduled = false;
			vm.ScheduleDate = futureDate;
			bool disabledDateInFuture = vm.GetErrors( nameof( ComposeTweetViewModel.ScheduleDate ) ).Cast<object>().Any();

			vm.IsTweetScheduled = true;
			vm.ScheduleDate = pastDate;
			bool enabledDateInPast = vm.GetErrors( nameof( ComposeTweetViewModel.ScheduleDate ) ).Cast<object>().Any();

			vm.IsTweetScheduled = true;
			vm.ScheduleDate = futureDate;
			bool enabledDateInFuture = vm.GetErrors( nameof( ComposeTweetViewModel.ScheduleDate ) ).Cast<object>().Any();

			// Assert
			Assert.IsFalse( disabledDateInPast );
			Assert.IsFalse( disabledDateInFuture );
			Assert.IsTrue( enabledDateInPast );
			Assert.IsFalse( enabledDateInFuture );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ScheduledTimeMustBeInFutureWhenEnabled()
		{
			// Arrange
			var vm = new ComposeTweetViewModel();

			var pastDate = DateTime.Now.AddHours( -1 );
			var futureDate = DateTime.Now.AddHours( 1 );

			// Act
			vm.IsTweetScheduled = false;
			vm.ScheduleTime = pastDate;
			bool disabledDateInPast = vm.GetErrors( nameof( ComposeTweetViewModel.ScheduleTime ) ).Cast<object>().Any();

			vm.IsTweetScheduled = false;
			vm.ScheduleTime = futureDate;
			bool disabledDateInFuture = vm.GetErrors( nameof( ComposeTweetViewModel.ScheduleTime ) ).Cast<object>().Any();

			vm.IsTweetScheduled = true;
			vm.ScheduleTime = pastDate;
			bool enabledDateInPast = vm.GetErrors( nameof( ComposeTweetViewModel.ScheduleTime ) ).Cast<object>().Any();

			vm.IsTweetScheduled = true;
			vm.ScheduleTime = futureDate;
			bool enabledDateInFuture = vm.GetErrors( nameof( ComposeTweetViewModel.ScheduleTime ) ).Cast<object>().Any();

			// Assert
			Assert.IsFalse( disabledDateInPast );
			Assert.IsFalse( disabledDateInFuture );
			Assert.IsTrue( enabledDateInPast );
			Assert.IsFalse( enabledDateInFuture );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ScheduleJobIsCreatedIfNeeded()
		{
			// Arrange
			Expression<Func<SchedulerJob, bool>> jobVerifier =
				job => job.JobType == SchedulerJobType.CreateStatus
						&& job.Text == "Hello World";

			var scheduler = new Mock<IScheduler>();
			scheduler.Setup( s => s.AddJob( It.Is( jobVerifier ) ) ).Verifiable();

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com/image.png" ) );

			var vm = new ComposeTweetViewModel
			{
				Scheduler = scheduler.Object,
				TwitterConfig = new Mock<ITwitterConfiguration>().Object,
				Dispatcher = new SyncDispatcher(),
				IsTweetScheduled = true,
				ScheduleDate = DateTime.Now.AddDays( 10 ),
				ScheduleTime = DateTime.Now.AddMinutes( 1 ),
				Text = "Hello World"
			};

			vm.Accounts.Add( new AccountEntry( context.Object, false ) {Use = true} );

			var waitHandle = new ManualResetEventSlim( false );
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( ComposeTweetViewModel.IsSending ) && !vm.IsSending )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.SendTweetCommand.Execute( null );

			// Assert
			Assert.IsTrue( waitHandle.Wait( 1000 ) );
			scheduler.Verify( s => s.AddJob( It.Is( jobVerifier ) ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void SendingTweetCallsTwitterApi()
		{
			// Arrange
			var waitHandle = new ManualResetEventSlim( false );
			var vm = new ComposeTweetViewModel
			{
				TwitterConfig = MockTwitterConfig(),
				Text = "Hello world"
			};

			var context = new Mock<IContextEntry>();
			var status = DummyGenerator.CreateDummyStatus();
			context.Setup( c => c.Twitter.Statuses.TweetAsync( "Hello world", It.IsAny<IEnumerable<ulong>>(), 0 ) ).Returns(
				Task.FromResult( status ) ).Verifiable();
			context.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com/image.png" ) );

			vm.Accounts.Add( new AccountEntry( context.Object, false ) {Use = true} );
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( ComposeTweetViewModel.IsSending ) && vm.IsSending == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.SendTweetCommand.Execute( null );
			waitHandle.Wait( 1000 );

			// Assert
			context.Verify( c => c.Twitter.Statuses.TweetAsync( "Hello world", It.IsAny<IEnumerable<ulong>>(), 0 ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void TweetCommandIsCorrectlyDisabled()
		{
			// Arrange
			var vm = new ComposeTweetViewModel
			{
				TwitterConfig = MockTwitterConfig()
			};

			bool requiresConfirmation = true;
			var contextEntry = new Mock<IContextEntry>();
			contextEntry.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com" ) );

			// ReSharper disable once AccessToModifiedClosure
			contextEntry.SetupGet( c => c.RequiresConfirmation ).Returns( () => requiresConfirmation );

			// Act
			var noData = vm.SendTweetCommand.CanExecute( null );

			vm.Text = "\r\n\t\t ";
			var onlyWhitespace = vm.SendTweetCommand.CanExecute( null );

			vm.Text = "test";
			vm.Accounts.Add( new AccountEntry( contextEntry.Object, false ) );
			var noUsedAccounts = vm.SendTweetCommand.CanExecute( null );

			vm.Accounts.First().Use = true;
			var noConfirmationSet = vm.SendTweetCommand.CanExecute( null );

			requiresConfirmation = false;
			vm.Text = new string( 'x', 141 );
			var tooLong = vm.SendTweetCommand.CanExecute( null );

			vm.Text = "test";
			var ok = vm.SendTweetCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( noData );
			Assert.IsFalse( onlyWhitespace );
			Assert.IsFalse( noUsedAccounts );
			Assert.IsFalse( noConfirmationSet );
			Assert.IsFalse( tooLong );
			Assert.IsTrue( ok );
		}

		private static ITwitterConfiguration MockTwitterConfig()
		{
			var cfg = new Mock<ITwitterConfiguration>();
			cfg.SetupGet( c => c.UrlLength ).Returns( 23 );
			cfg.SetupGet( c => c.UrlLengthHttps ).Returns( 23 );

			return cfg.Object;
		}
	}
}