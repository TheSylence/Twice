using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Cache;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.Services.Views;
using Twice.ViewModels;
using Twice.ViewModels.Twitter;

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

			vm.Accounts.Add( new AccountEntry( context.Object ) {Use = true} );

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
			vm.Accounts.Add( new AccountEntry( context.Object ) {Use = true} );

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
			await vm.OnLoad( null );

			// Assert
			var usedAccount = vm.Accounts.SingleOrDefault( a => a.Use );
			Assert.IsNotNull( usedAccount );

			var notUsedAccount = vm.Accounts.SingleOrDefault( a => !a.Use );
			Assert.IsNotNull( notUsedAccount );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void MediaCanBeRemoved()
		{
			// Arrange
			var vm = new ComposeTweetViewModel();
			vm.AttachedMedias.Add( new MediaItem( 123, new byte[] {} ) );

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
		public void MimeTypeForFileIsDetectedCorrectly()
		{
			// Arrange
			var testCases = new Dictionary<string, string>
			{
				{"file.jpg", "application/octet-stream"},
				{"file.png", "image/png"},
				{"file.bmp", "image/bmp"},
				{"file.gif", "image/gif"},
				{"file.png.gif", "image/gif"},
				{"file", "application/octet-stream"},
				{"name.exe", "application/octet-stream"},
				{"name", "application/octet-stream"}
			};

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp => ComposeTweetViewModel.GetMimeType( kvp.Key ) );

			// Assert
			foreach( var kvp in results )
			{
				Assert.AreEqual( testCases[kvp.Key], kvp.Value );
			}
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
			tester.Test( nameof( ComposeTweetViewModel.Notifier ), nameof( ComposeTweetViewModel.Cache ) );

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
			context.Setup( c => c.Twitter.TweetAsync( "Hello world " + url, It.IsAny<IEnumerable<ulong>>() ) ).Returns(
				Task.FromResult( status ) ).Verifiable();
			context.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com/image.png" ) );

			var waitHandle = new ManualResetEventSlim( false );
			var vm = new ComposeTweetViewModel
			{
				TwitterConfig = MockTwitterConfig(),
				Text = "Hello world",
				QuotedTweet = new StatusViewModel( quotedTweet, context.Object, config.Object, viewServiceRepo.Object )
			};

			vm.Accounts.Add( new AccountEntry( context.Object ) {Use = true} );
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
			context.Verify( c => c.Twitter.TweetAsync( "Hello world " + url, It.IsAny<IEnumerable<ulong>>() ), Times.Once() );
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
			context.Setup( c => c.Twitter.TweetAsync( "Hello world", It.IsAny<IEnumerable<ulong>>() ) ).Returns(
				Task.FromResult( status ) ).Verifiable();
			context.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com/image.png" ) );

			vm.Accounts.Add( new AccountEntry( context.Object ) {Use = true} );
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
			context.Verify( c => c.Twitter.TweetAsync( "Hello world", It.IsAny<IEnumerable<ulong>>() ), Times.Once() );
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
			vm.Accounts.Add( new AccountEntry( contextEntry.Object ) );
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