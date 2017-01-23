using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;

namespace Twice.Tests.Models.Twitter.Streaming
{
	[TestClass, ExcludeFromCodeCoverage]
	public class StreamParserTests
	{
		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public async Task ConnectionIsClosedOnDispose()
		{
			// Arrange
			var stream = new Mock<IStreaming>();
			stream.Setup( s => s.CloseStream() ).Verifiable();

			var streamingList = new List<IStreaming>
			{
				stream.Object
			};

			var userStream = new Mock<IStreamingConnection>();
			userStream.Setup( s => s.Start( It.IsAny<Func<IStreamContent, Task>>() ) ).Returns( Task.FromResult( streamingList ) );

			var parser = StreamParser.Create( userStream.Object, null );

			parser.StartStreaming();

			// Act
			await parser.StreamingTask;
			parser.Dispose();

			// Assert
			stream.Verify( s => s.CloseStream(), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public async Task KeepAliveMessageIsIgnored()
		{
			// Arrange
			const string strContent = "";
			var parser = SetupParser( strContent );

			// Act
			parser.StartStreaming();
			await parser.StreamingTask;

			// Assert
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingExtendedTweetRaisesEventWithCorrectData()
		{
			// Arrange
			const string strContent = "{\"created_at\":\"Thu Dec 08 17:28:54 +0000 2016\",\"id\":806913456204955648,\"id_str\":\"806913456204955648\",\"text\":\"Das ist mal wieder ein #Test. Und jetzt muss ich sogar irgendwie dieses mal die 140 Zeichen vollbekommen. Aber wie\\u2026 https:\\/\\/t.co\\/C5faezUuJ5\",\"display_text_range\":[0,140],\"source\":\"\\u003ca href=\\\"http:\\/\\/twitter.com\\\" rel=\\\"nofollow\\\"\\u003eTwitter Web Client\\u003c\\/a\\u003e\",\"truncated\":true,\"in_reply_to_status_id\":null,\"in_reply_to_status_id_str\":null,\"in_reply_to_user_id\":null,\"in_reply_to_user_id_str\":null,\"in_reply_to_screen_name\":null,\"user\":{\"id\":333290820,\"id_str\":\"333290820\",\"name\":\"Sylence\",\"screen_name\":\"TheSylence\",\"location\":\"Zweiter Stern von rechts\",\"url\":\"http:\\/\\/btbsoft.org\",\"description\":\"Koffeinjunkie, Coda, Gamer, Nerd, #foreveralone mit @FemmeNoireOnYT \\u2665\\u2665\\u2665 Ach und hab ich erw\\u00e4hnt das ich einen Gottkomplex hab? ^^\",\"protected\":false,\"verified\":false,\"followers_count\":249,\"friends_count\":155,\"listed_count\":9,\"favourites_count\":310,\"statuses_count\":14223,\"created_at\":\"Mon Jul 11 09:13:20 +0000 2011\",\"utc_offset\":3600,\"time_zone\":\"Amsterdam\",\"geo_enabled\":false,\"lang\":\"de\",\"contributors_enabled\":false,\"is_translator\":false,\"profile_background_color\":\"C0DEED\",\"profile_background_image_url\":\"http:\\/\\/abs.twimg.com\\/images\\/themes\\/theme1\\/bg.png\",\"profile_background_image_url_https\":\"https:\\/\\/abs.twimg.com\\/images\\/themes\\/theme1\\/bg.png\",\"profile_background_tile\":false,\"profile_link_color\":\"1DA1F2\",\"profile_sidebar_border_color\":\"C0DEED\",\"profile_sidebar_fill_color\":\"DDEEF6\",\"profile_text_color\":\"333333\",\"profile_use_background_image\":true,\"profile_image_url\":\"http:\\/\\/pbs.twimg.com\\/profile_images\\/3486265195\\/7d5cc9fb1c7423677ae0fdfaa8b9b998_normal.png\",\"profile_image_url_https\":\"https:\\/\\/pbs.twimg.com\\/profile_images\\/3486265195\\/7d5cc9fb1c7423677ae0fdfaa8b9b998_normal.png\",\"default_profile\":true,\"default_profile_image\":false,\"following\":null,\"follow_request_sent\":null,\"notifications\":null},\"geo\":null,\"coordinates\":null,\"place\":null,\"contributors\":null,\"is_quote_status\":false,\"extended_tweet\":{\"full_text\":\"Das ist mal wieder ein #Test. Und jetzt muss ich sogar irgendwie dieses mal die 140 Zeichen vollbekommen. Aber wie in einem #test? Fragen :o https:\\/\\/t.co\\/huX4AXpa5f\",\"display_text_range\":[0,140],\"entities\":{\"hashtags\":[{\"text\":\"Test\",\"indices\":[23,28]},{\"text\":\"test\",\"indices\":[124,129]}],\"urls\":[],\"user_mentions\":[],\"symbols\":[],\"media\":[{\"id\":806913424777166852,\"id_str\":\"806913424777166852\",\"indices\":[141,164],\"media_url\":\"http:\\/\\/pbs.twimg.com\\/media\\/CzK7fBBWQAQpcbQ.jpg\",\"media_url_https\":\"https:\\/\\/pbs.twimg.com\\/media\\/CzK7fBBWQAQpcbQ.jpg\",\"url\":\"https:\\/\\/t.co\\/huX4AXpa5f\",\"display_url\":\"pic.twitter.com\\/huX4AXpa5f\",\"expanded_url\":\"https:\\/\\/twitter.com\\/TheSylence\\/status\\/806913456204955648\\/photo\\/1\",\"type\":\"photo\",\"sizes\":{\"thumb\":{\"w\":34,\"h\":34,\"resize\":\"crop\"},\"small\":{\"w\":59,\"h\":34,\"resize\":\"fit\"},\"large\":{\"w\":59,\"h\":34,\"resize\":\"fit\"},\"medium\":{\"w\":59,\"h\":34,\"resize\":\"fit\"}}}]},\"extended_entities\":{\"media\":[{\"id\":806913424777166852,\"id_str\":\"806913424777166852\",\"indices\":[141,164],\"media_url\":\"http:\\/\\/pbs.twimg.com\\/media\\/CzK7fBBWQAQpcbQ.jpg\",\"media_url_https\":\"https:\\/\\/pbs.twimg.com\\/media\\/CzK7fBBWQAQpcbQ.jpg\",\"url\":\"https:\\/\\/t.co\\/huX4AXpa5f\",\"display_url\":\"pic.twitter.com\\/huX4AXpa5f\",\"expanded_url\":\"https:\\/\\/twitter.com\\/TheSylence\\/status\\/806913456204955648\\/photo\\/1\",\"type\":\"photo\",\"sizes\":{\"thumb\":{\"w\":34,\"h\":34,\"resize\":\"crop\"},\"small\":{\"w\":59,\"h\":34,\"resize\":\"fit\"},\"large\":{\"w\":59,\"h\":34,\"resize\":\"fit\"},\"medium\":{\"w\":59,\"h\":34,\"resize\":\"fit\"}}}]}},\"retweet_count\":0,\"favorite_count\":0,\"entities\":{\"hashtags\":[{\"text\":\"Test\",\"indices\":[23,28]}],\"urls\":[{\"url\":\"https:\\/\\/t.co\\/C5faezUuJ5\",\"expanded_url\":\"https:\\/\\/twitter.com\\/i\\/web\\/status\\/806913456204955648\",\"display_url\":\"twitter.com\\/i\\/web\\/status\\/8\\u2026\",\"indices\":[116,139]}],\"user_mentions\":[],\"symbols\":[]},\"favorited\":false,\"retweeted\":false,\"possibly_sensitive\":false,\"filter_level\":\"low\",\"lang\":\"de\",\"timestamp_ms\":\"1481218134343\"}";

			ManualResetEventSlim waitHandle = new ManualResetEventSlim( false );
			var parser = SetupParser( strContent );
			StatusStreamEventArgs receivedData = null;

			parser.StatusReceived += ( s, e ) =>
			{
				receivedData = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.Wait( 1000 );

			// Assert
			Assert.IsNotNull( receivedData );
			Assert.IsTrue( receivedData.Status.Text.StartsWith( "Das ist mal wieder ein #Test. Und jetzt muss ich sogar irgendwie dieses mal die 140 Zeichen vollbekommen. Aber wie in einem #test? Fragen :o" ) );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingFavoriteRaisesEvent()
		{
			// Arrange
			const string strContent =
				"{ \"event\":\"favorite\", \"created_at\": \"Sat Sep 04 16:10:54 +0000 2010\", \"target\": { \"id\": 123 }, \"source\": { \"id\": 456 }, \"target_object\": { \"created_at\": \"Wed Jun 06 20:07:10 +0000 2012\", \"id_str\": \"210462857140252672\" } }";

			ManualResetEventSlim waitHandle = new ManualResetEventSlim( false );
			var parser = SetupParser( strContent );
			EventStreamEventArgs receivedData = null;

			parser.FavoriteEventReceived += ( s, e ) =>
			{
				receivedData = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.Wait( 1000 );

			// Assert
			Assert.IsNotNull( receivedData );
			Assert.AreEqual( StreamEventType.Favorite, receivedData.Event );
			Assert.AreEqual( 456ul, receivedData.Source.GetUserId() );
			Assert.AreEqual( 123ul, receivedData.Target.GetUserId() );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingFriendlistRaisesEvent()
		{
			// Arrange
			const string strContent = "{\"friends\":[123,456,789]}";

			ManualResetEventSlim waitHandle = new ManualResetEventSlim( false );
			var parser = SetupParser( strContent );
			FriendsStreamEventArgs receivedData = null;

			parser.FriendsReceived += ( s, e ) =>
			{
				receivedData = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.Wait( 1000 );

			// Assert
			Assert.IsNotNull( receivedData );
			CollectionAssert.AreEqual( new ulong[] { 123, 456, 789 }, receivedData.Friends );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingMessageDeleteRaisesEvent()
		{
			// Arrange
			const string strContent =
				"{\"delete\":{\"direct_message\":{\"id\":1234,\"id_str\":\"1234\",\"user_id\":3,\"user_id_str\":\"3\"}}}";

			ManualResetEventSlim waitHandle = new ManualResetEventSlim( false );
			var parser = SetupParser( strContent );
			DeleteStreamEventArgs receivedDelete = null;

			parser.DirectMessageDeleted += ( s, e ) =>
			{
				receivedDelete = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.Wait( 1000 );

			// Assert
			Assert.IsNotNull( receivedDelete );
			Assert.AreEqual( 1234ul, receivedDelete.Id );
			Assert.AreEqual( 3ul, receivedDelete.UserId );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingMessageRaisesEvent()
		{
			// Arrange
			var strContent = File.ReadAllText( "Data/message.json" );

			ManualResetEventSlim waitHandle = new ManualResetEventSlim( false );
			var parser = SetupParser( strContent );
			DirectMessageStreamEventArgs receivedData = null;

			parser.DirectMessageReceived += ( s, e ) =>
			{
				receivedData = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.Wait( 1000 );

			// Assert
			Assert.IsNotNull( receivedData );
			Assert.AreNotEqual( 0ul, receivedData.Message.GetMessageId() );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingStatusDeleteRaisesEvent()
		{
			// Arrange
			const string strContent = "{\"delete\":{\"status\":{\"id\":1234,\"id_str\":\"1234\",\"user_id\":3,\"user_id_str\":\"3\"}}}";

			ManualResetEventSlim waitHandle = new ManualResetEventSlim( false );
			var parser = SetupParser( strContent );
			DeleteStreamEventArgs receivedDelete = null;

			parser.StatusDeleted += ( s, e ) =>
			{
				receivedDelete = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.Wait( 1000 );

			// Assert
			Assert.IsNotNull( receivedDelete );
			Assert.AreEqual( 1234ul, receivedDelete.Id );
			Assert.AreEqual( 3ul, receivedDelete.UserId );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingStatusRaisesEvent()
		{
			// Arrange
			var strContent = File.ReadAllText( "Data/tweet.json" );

			ManualResetEventSlim waitHandle = new ManualResetEventSlim( false );
			var parser = SetupParser( strContent );
			StatusStreamEventArgs receivedData = null;

			parser.StatusReceived += ( s, e ) =>
			{
				receivedData = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.Wait( 1000 );

			// Assert
			Assert.IsNotNull( receivedData );
			Assert.AreNotEqual( 0ul, receivedData.Status.StatusID );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingUnknownDataRaisesEvent()
		{
			// Arrange
			const string strContent = "{\"test\":[1,2,3]}";

			ManualResetEventSlim waitHandle = new ManualResetEventSlim( false );
			var parser = SetupParser( strContent );
			StreamEventArgs receivedData = null;

			parser.UnknownDataReceived += ( s, e ) =>
			{
				receivedData = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.Wait( 1000 );

			// Assert
			Assert.IsNotNull( receivedData );
			Assert.AreEqual( strContent, receivedData.Json );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingUnkownDataRaisesEvent()
		{
			// Arrange
			const string strContent =
				"{ \"event\":\"unknown\", \"created_at\": \"Sat Sep 04 16:10:54 +0000 2010\", \"target\": { \"id\": 123 }, \"source\": { \"id\": 456 }, \"target_object\": { \"created_at\": \"Wed Jun 06 20:07:10 +0000 2012\", \"id_str\": \"210462857140252672\" } }";

			ManualResetEventSlim waitHandle = new ManualResetEventSlim( false );
			var parser = SetupParser( strContent );
			EventStreamEventArgs receivedData = null;

			parser.UnknownEventReceived += ( s, e ) =>
			{
				receivedData = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.Wait( 1000 );

			// Assert
			Assert.IsNotNull( receivedData );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void StreamIsNotStartedTwice()
		{
			// Arrange
			var userStream = new Mock<IStreamingConnection>();
			userStream.Setup( s => s.Start( It.IsAny<Func<IStreamContent, Task>>() ) )
				.Returns( Task.FromResult( new List<IStreaming>() ) ).Verifiable();

			var parser = StreamParser.Create( userStream.Object, null );

			// Act
			parser.StartStreaming();
			parser.StartStreaming();

			// Assert
			userStream.Verify( s => s.Start( It.IsAny<Func<IStreamContent, Task>>() ), Times.AtMostOnce() );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void StreamParserCanBeCreated()
		{
			// Arrange
			var userStream = new Mock<IStreamingConnection>();

			// Act
			var parser = StreamParser.Create( userStream.Object, null );

			// Assert
			Assert.IsNotNull( parser );
		}

		private static StreamParser SetupParser( string strContent )
		{
			var execute = new Mock<ITwitterExecute>();
			StreamContent content = new StreamContent( execute.Object, strContent );

			var stream = new Mock<IStreamingConnection>();
			stream.Setup( s => s.Start( It.IsAny<Func<IStreamContent, Task>>() ) )
				.Callback<Func<StreamContent, Task>>( func => func( content ) )
				.Returns( Task.FromResult( new List<IStreaming>() ) );

			var parser = StreamParser.Create( stream.Object, null );
			return parser;
		}
	}
}