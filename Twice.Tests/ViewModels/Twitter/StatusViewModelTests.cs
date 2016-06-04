using LinqToTwitter;
using LitJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twice.Models.Configuration;
using Twice.Models.Media;
using Twice.Models.Twitter;
using Twice.Services.Views;
using Twice.Utilities.Os;
using Twice.ViewModels;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
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
			var vm = new StatusViewModel( status, context.Object, null, null );

			var clipboard = new Mock<IClipboard>();
			clipboard.Setup( c => c.SetText( It.Is<string>( str => Uri.IsWellFormedUriString( str, UriKind.Absolute ) ) ) )
				.Verifiable();
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
			var vm = new StatusViewModel( status, context.Object, null, null );

			var clipboard = new Mock<IClipboard>();
			clipboard.Setup( c => c.SetText( "@Testi: hello world" ) ).Verifiable();
			vm.Clipboard = clipboard.Object;

			// Act
			vm.CopyTweetCommand.Execute( null );

			// Assert
			clipboard.Verify( c => c.SetText( "@Testi: hello world" ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void CreationDateIsCorrectlyExtracted()
		{
			// Arrange
			var status = DummyGenerator.CreateDummyStatus();
			status.CreatedAt = new DateTime( 1, 2, 3, 4, 5, 6 );

			var vm = new StatusViewModel( status, null, null, null );

			// Act
			var created = vm.CreatedAt;

			// Assert
			Assert.AreEqual( status.CreatedAt, created );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void DeletingStatusCallsTwitterApi()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );
			context.Setup( c => c.Notifier.DisplayMessage( It.IsAny<string>(), NotificationType.Success ) ).Verifiable();
			context.Setup( c => c.Twitter.DeleteTweetAsync( 456 ) ).Returns( Task.FromResult<Status>( null ) ).Verifiable();

			var waitHandle = new ManualResetEventSlim( false );
			var status = DummyGenerator.CreateDummyStatus();
			status.StatusID = 456;
			status.User.UserID = 123;
			var vm = new StatusViewModel( status, context.Object, null, null )
			{
				Dispatcher = new SyncDispatcher()
			};
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( vm.IsLoading ) && vm.IsLoading == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.DeleteStatusCommand.Execute( null );
			waitHandle.Wait( 1000 );

			// Assert
			context.Verify( c => c.Twitter.DeleteTweetAsync( 456 ), Times.Once() );
			context.Verify( c => c.Notifier.DisplayMessage( It.IsAny<string>(), NotificationType.Success ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ExtendedMediasAreIncludedInInlineMedias()
		{
			// Arrange
			var json = File.ReadAllText( "Data/tweet_extmedia.json" );
			var data = JsonMapper.ToObject( json );
			var status = new Status( data );

			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			var visualConfig = new VisualConfig { InlineMedia = true };
			config.SetupGet( c => c.Visual ).Returns( visualConfig );
			var vm = new StatusViewModel( status, context.Object, config.Object, null );

			// Act
			var medias = vm.InlineMedias.ToArray();

			// Assert
			Assert.AreEqual( 3, medias.Length );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ForeignStatusCanBeQuoted()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 222;
			var vm = new StatusViewModel( status, context.Object, null, null );

			// Act
			bool canExecute = vm.QuoteStatusCommand.CanExecute( null );

			// Assert
			Assert.IsTrue( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ForeignStatusCanBeReportedAsSpam()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 222;
			var vm = new StatusViewModel( status, context.Object, null, null );

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
			var vm = new StatusViewModel( status, context.Object, null, null );

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
			var vm = new StatusViewModel( status, context.Object, null, null );

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
			var vm = new StatusViewModel( status, context.Object, null, null );

			// Act
			bool canExecute = vm.BlockUserCommand.CanExecute( null );

			// Assert
			Assert.IsTrue( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void InlineMediasAreExtractedCorrectly()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 222;
			status.Entities.MediaEntities.Add( new MediaEntity { MediaUrlHttps = "https://example.com/1", ID = 1 } );
			status.Entities.MediaEntities.Add( new MediaEntity { MediaUrlHttps = "https://example.com/2", ID = 2 } );
			status.Entities.MediaEntities.Add( new MediaEntity { MediaUrlHttps = "https://example.com/3", ID = 3 } );

			var config = new Mock<IConfig>();
			var visualConfig = new VisualConfig { InlineMedia = true };
			config.SetupGet( c => c.Visual ).Returns( visualConfig );

			// Act
			var vm = new StatusViewModel( status, context.Object, config.Object, null );
			var medias = vm.InlineMedias.ToArray();

			// Assert
			Assert.AreEqual( 3, medias.Length );
			Assert.IsNotNull( medias.SingleOrDefault( m => m.Url.AbsoluteUri == "https://example.com/1" ) );
			Assert.IsNotNull( medias.SingleOrDefault( m => m.Url.AbsoluteUri == "https://example.com/2" ) );
			Assert.IsNotNull( medias.SingleOrDefault( m => m.Url.AbsoluteUri == "https://example.com/3" ) );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void InlineMediasContainExtractedMedias()
		{
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 222;
			status.Entities.UrlEntities.Add( new UrlEntity { ExpandedUrl = "https://example.com/1" } );
			status.Entities.UrlEntities.Add( new UrlEntity { ExpandedUrl = "https://example.com/2" } );
			status.Entities.UrlEntities.Add( new UrlEntity { ExpandedUrl = "https://example.com/3" } );

			var config = new Mock<IConfig>();
			var visualConfig = new VisualConfig { InlineMedia = true };
			config.SetupGet( c => c.Visual ).Returns( visualConfig );

			var extractorRepo = new Mock<IMediaExtractorRepository>();
			extractorRepo.Setup( r => r.ExtractMedia( It.IsAny<string>() ) ).Returns<string>( url => new Uri( url ) );

			var vm = new StatusViewModel( status, context.Object, config.Object, null )
			{
				MediaExtractor = extractorRepo.Object
			};

			// Act
			var medias = vm.InlineMedias.ToArray();

			// Assert
			Assert.AreEqual( 3, medias.Length );
			Assert.IsNotNull( medias.SingleOrDefault( m => m.Url.AbsoluteUri == "https://example.com/1" ) );
			Assert.IsNotNull( medias.SingleOrDefault( m => m.Url.AbsoluteUri == "https://example.com/2" ) );
			Assert.IsNotNull( medias.SingleOrDefault( m => m.Url.AbsoluteUri == "https://example.com/3" ) );

			extractorRepo.Verify( e => e.ExtractMedia( It.IsAny<string>() ), Times.Exactly( 3 ) );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void InlineMediaUseExtendedEntities()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 222;
			status.Entities.MediaEntities.Add( new MediaEntity { MediaUrlHttps = "https://example.com/1", ID = 1 } );
			status.Entities.MediaEntities.Add( new MediaEntity { MediaUrlHttps = "https://example.com/2", ID = 2 } );
			status.ExtendedEntities.MediaEntities.Add( new MediaEntity { MediaUrlHttps = "https://example.com/3", ID = 3 } );

			var config = new Mock<IConfig>();
			var visualConfig = new VisualConfig { InlineMedia = true };
			config.SetupGet( c => c.Visual ).Returns( visualConfig );

			// Act
			var vm = new StatusViewModel( status, context.Object, config.Object, null );
			var medias = vm.InlineMedias.ToArray();

			// Assert
			Assert.AreEqual( 3, medias.Length );
			Assert.IsNotNull( medias.SingleOrDefault( m => m.Url.AbsoluteUri == "https://example.com/1" ) );
			Assert.IsNotNull( medias.SingleOrDefault( m => m.Url.AbsoluteUri == "https://example.com/2" ) );
			Assert.IsNotNull( medias.SingleOrDefault( m => m.Url.AbsoluteUri == "https://example.com/3" ) );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void OwnStatusCanBeDeleted()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 123;
			var vm = new StatusViewModel( status, context.Object, null, null );

			// Act
			bool canExecute = vm.DeleteStatusCommand.CanExecute( null );

			// Assert
			Assert.IsTrue( canExecute );
		}

		[TestMethod, TestCategory( "ViewMoels.Twitter" )]
		public void OwnStatusCanBeQuoted()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 123;
			var vm = new StatusViewModel( status, context.Object, null, null );

			// Act
			bool canExecute = vm.QuoteStatusCommand.CanExecute( null );

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
			var vm = new StatusViewModel( status, context.Object, null, null );

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
			var vm = new StatusViewModel( status, context.Object, null, null );

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
			var vm = new StatusViewModel( status, context.Object, null, null );

			// Act
			bool canExecute = vm.BlockUserCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void QuoteIsExtractedFromStatus()
		{
			// Arrange
			var status = DummyGenerator.CreateDummyStatus();
			status.Entities.UrlEntities.Add( new UrlEntity { ExpandedUrl = "https://twitter.com/user/status/123456" } );

			var vm = new StatusViewModel( status, null, null, null );

			// Act
			ulong quotedId = vm.ExtractQuotedTweetUrl();
			bool hasQuote = vm.HasQuotedTweet;

			// Assert
			Assert.AreEqual( 123456ul, quotedId );
			Assert.IsTrue( hasQuote );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void QuoteIsNotExtractedForDifferentUrls()
		{
			// Arrange
			var status = DummyGenerator.CreateDummyStatus();
			status.Entities.UrlEntities.Add( new UrlEntity { ExpandedUrl = "https://example.com/123456" } );

			var vm = new StatusViewModel( status, null, null, null );

			// Act
			ulong quotedId = vm.ExtractQuotedTweetUrl();
			bool hasQuote = vm.HasQuotedTweet;

			// Assert
			Assert.AreEqual( 0ul, quotedId );
			Assert.IsFalse( hasQuote );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void QuotingTweetOpensDialog()
		{
			// Arrange
			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.QuoteTweet( It.IsAny<StatusViewModel>(), null ) ).Returns( Task.CompletedTask ).Verifiable();

			var vm = new StatusViewModel( DummyGenerator.CreateDummyStatus(), null, null, viewServices.Object );

			// Act
			vm.QuoteStatusCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.QuoteTweet( It.IsAny<StatusViewModel>(), null ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ReplyCanAlwaysBeExecuted()
		{
			// Arrange
			var status = DummyGenerator.CreateDummyStatus();
			var context = new Mock<IContextEntry>();

			var vm = new StatusViewModel( status, context.Object, null, null );

			// Act
			bool canExecute = vm.ReplyCommand.CanExecute( null );

			// Assert
			Assert.IsTrue( canExecute );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ReplyToAllNeedsAtLeastTwoUsers()
		{
			// Arrange
			var context = new Mock<IContextEntry>();

			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 123;
			status.Entities = new Entities
			{
				UserMentionEntities = new List<UserMentionEntity>
				{
					new UserMentionEntity {Id = 123}
				}
			};
			var vm = new StatusViewModel( status, context.Object, null, null );

			// Act
			bool single = vm.ReplyToAllCommand.CanExecute( null );
			status.User.UserID = 222;
			bool multiple = vm.ReplyToAllCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( single );
			Assert.IsTrue( multiple );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void RetweetsAreHandledCorrectly()
		{
			// Arrange
			var origUser = DummyGenerator.CreateDummyUser();
			origUser.UserID = 11;

			var originalStatus = DummyGenerator.CreateDummyStatus( origUser );
			originalStatus.ID = originalStatus.StatusID = 1;

			var rtUser = DummyGenerator.CreateDummyUser();
			rtUser.UserID = 22;
			var retweet = DummyGenerator.CreateDummyStatus( rtUser );
			retweet.RetweetedStatus = originalStatus;
			retweet.ID = retweet.StatusID = 2;

			// Act
			var vm = new StatusViewModel( retweet, null, null, null );

			// Assert
			Assert.AreEqual( 22ul, vm.SourceUser.UserId );
			Assert.AreEqual( 11ul, vm.User.UserId );

			Assert.AreEqual( 1ul, vm.Model.ID );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void StatusCanBeFavorited()
		{
			// Arrange
			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 111;
			status.StatusID = 12345;

			var twitter = new Mock<ITwitterContext>();
			twitter.Setup( c => c.CreateFavoriteAsync( 12345 ) ).Returns( Task.FromResult( status ) ).Verifiable();

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.Twitter ).Returns( twitter.Object );
			context.SetupGet( c => c.UserId ).Returns( 123 );
			var waitHandle = new ManualResetEventSlim( false );
			var vm = new StatusViewModel( status, context.Object, null, null )
			{
				Dispatcher = new SyncDispatcher()
			};
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( vm.IsLoading ) && vm.IsLoading == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.FavoriteStatusCommand.Execute( null );
			waitHandle.Wait( 1000 );

			// Assert
			twitter.Verify( t => t.CreateFavoriteAsync( 12345 ), Times.Once() );
			Assert.IsTrue( status.Favorited );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void StatusCanBeUnfavorited()
		{
			// Arrange
			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 111;
			status.StatusID = 12345;
			status.Favorited = true;

			var twitter = new Mock<ITwitterContext>();
			twitter.Setup( c => c.DestroyFavoriteAsync( 12345 ) ).Returns( Task.FromResult( status ) ).Verifiable();

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.Twitter ).Returns( twitter.Object );
			context.SetupGet( c => c.UserId ).Returns( 123 );
			var waitHandle = new ManualResetEventSlim( false );
			var vm = new StatusViewModel( status, context.Object, null, null )
			{
				Dispatcher = new SyncDispatcher()
			};
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( vm.IsLoading ) && vm.IsLoading == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.FavoriteStatusCommand.Execute( null );
			waitHandle.Wait( 1000 );

			// Assert
			twitter.Verify( t => t.DestroyFavoriteAsync( 12345 ), Times.Once() );
			Assert.IsFalse( status.Favorited );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void UserCanBeBlocked()
		{
			// Arrange
			var status = DummyGenerator.CreateDummyStatus();
			status.User.UserID = 111;
			status.UserID = 111;

			var twitter = new Mock<ITwitterContext>();
			twitter.Setup( t => t.CreateBlockAsync( 111, It.IsAny<string>(), It.IsAny<bool>() ) ).Returns(
				Task.FromResult( status.User ) ).Verifiable();

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.Twitter ).Returns( twitter.Object );
			context.SetupGet( c => c.UserId ).Returns( 123 );
			var waitHandle = new ManualResetEventSlim( false );
			var vm = new StatusViewModel( status, context.Object, null, null )
			{
				Dispatcher = new SyncDispatcher()
			};
			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( vm.IsLoading ) && vm.IsLoading == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.BlockUserCommand.Execute( null );
			waitHandle.Wait( 1000 );

			// Assert
			twitter.Verify( t => t.CreateBlockAsync( 111, It.IsAny<string>(), It.IsAny<bool>() ), Times.Once() );
		}
	}
}