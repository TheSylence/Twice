using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Twice.Models.Twitter.Streaming;

namespace Twice.Tests.Models.Twitter.Streaming
{
	[TestClass]
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
			userStream.Setup( s => s.Start( It.IsAny<Func<StreamContent, Task>>() ) ).Returns( Task.FromResult( streamingList ) );

			var parser = StreamParser.Create( userStream.Object );

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
			string strContent = "";
			var execute = new Mock<ITwitterExecute>();
			StreamContent content = new StreamContent( execute.Object, strContent );

			var stream = new Mock<IStreamingConnection>();
			stream.Setup( s => s.Start( It.IsAny<Func<StreamContent, Task>>() ) )
				.Callback<Func<StreamContent, Task>>( func => func( content ) )
				.Returns( Task.FromResult( new List<IStreaming>() ) );

			var parser = StreamParser.Create( stream.Object );

			// Act
			parser.StartStreaming();
			await parser.StreamingTask;

			// Assert
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingFriendlistRaisesEvent()
		{
			// Arrange
			string strContent = "{\"friends\":[123,456,789]}";
			var execute = new Mock<ITwitterExecute>();
			StreamContent content = new StreamContent( execute.Object, strContent );

			var stream = new Mock<IStreamingConnection>();
			stream.Setup( s => s.Start( It.IsAny<Func<StreamContent, Task>>() ) )
				.Callback<Func<StreamContent, Task>>( func => func( content ) )
				.Returns( Task.FromResult( new List<IStreaming>() ) );

			var parser = StreamParser.Create( stream.Object );
			FriendsStreamEventArgs receivedData = null;
			ManualResetEvent waitHandle = new ManualResetEvent( false );

			parser.FriendsReceived += ( s, e ) =>
			{
				receivedData = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.WaitOne();

			// Assert
			Assert.IsNotNull( receivedData );
			CollectionAssert.AreEqual( new ulong[] { 123, 456, 789 }, receivedData.Friends );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingMessageDeleteRaisesEvent()
		{
			// Arrange
			string strContent = "{\"delete\":{\"direct_message\":{\"id\":1234,\"id_str\":\"1234\",\"user_id\":3,\"user_id_str\":\"3\"}}}";
			var execute = new Mock<ITwitterExecute>();
			StreamContent content = new StreamContent( execute.Object, strContent );

			var stream = new Mock<IStreamingConnection>();
			stream.Setup( s => s.Start( It.IsAny<Func<StreamContent, Task>>() ) )
				.Callback<Func<StreamContent, Task>>( func => func( content ) )
				.Returns( Task.FromResult( new List<IStreaming>() ) );

			var parser = StreamParser.Create( stream.Object );
			DeleteStreamEventArgs receivedDelete = null;
			ManualResetEvent waitHandle = new ManualResetEvent( false );

			parser.DirectMessageDeleted += ( s, e ) =>
			{
				receivedDelete = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.WaitOne();

			// Assert
			Assert.IsNotNull( receivedDelete );
			Assert.AreEqual( 1234ul, receivedDelete.Id );
			Assert.AreEqual( 3ul, receivedDelete.UserId );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingStatusDeleteRaisesEvent()
		{
			// Arrange
			string strContent = "{\"delete\":{\"status\":{\"id\":1234,\"id_str\":\"1234\",\"user_id\":3,\"user_id_str\":\"3\"}}}";
			var execute = new Mock<ITwitterExecute>();
			StreamContent content = new StreamContent( execute.Object, strContent );

			var stream = new Mock<IStreamingConnection>();
			stream.Setup( s => s.Start( It.IsAny<Func<StreamContent, Task>>() ) )
				.Callback<Func<StreamContent, Task>>( func => func( content ) )
				.Returns( Task.FromResult( new List<IStreaming>() ) );

			var parser = StreamParser.Create( stream.Object );
			DeleteStreamEventArgs receivedDelete = null;
			ManualResetEvent waitHandle = new ManualResetEvent( false );

			parser.StatusDeleted += ( s, e ) =>
			{
				receivedDelete = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.WaitOne();

			// Assert
			Assert.IsNotNull( receivedDelete );
			Assert.AreEqual( 1234ul, receivedDelete.Id );
			Assert.AreEqual( 3ul, receivedDelete.UserId );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ReceivingUnknownDataRaisesEvent()
		{
			// Arrange
			string strContent = "{\"test\":[1,2,3]}";
			var execute = new Mock<ITwitterExecute>();
			StreamContent content = new StreamContent( execute.Object, strContent );

			var stream = new Mock<IStreamingConnection>();
			stream.Setup( s => s.Start( It.IsAny<Func<StreamContent, Task>>() ) )
				.Callback<Func<StreamContent, Task>>( func => func( content ) )
				.Returns( Task.FromResult( new List<IStreaming>() ) );

			var parser = StreamParser.Create( stream.Object );
			StreamEventArgs receivedData = null;
			ManualResetEvent waitHandle = new ManualResetEvent( false );

			parser.UnknownDataReceived += ( s, e ) =>
			{
				receivedData = e;
				waitHandle.Set();
			};

			// Act
			parser.StartStreaming();
			waitHandle.WaitOne();

			// Assert
			Assert.IsNotNull( receivedData );
			Assert.AreEqual( strContent, receivedData.Json );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void StreamIsNotStartedTwice()
		{
			// Arrange
			var userStream = new Mock<IStreamingConnection>();
			userStream.Setup( s => s.Start( It.IsAny<Func<StreamContent, Task>>() ) )
				.Returns( Task.FromResult( new List<IStreaming>() ) ).Verifiable();

			var parser = StreamParser.Create( userStream.Object );

			// Act
			parser.StartStreaming();
			parser.StartStreaming();

			// Assert
			userStream.Verify( s => s.Start( It.IsAny<Func<StreamContent, Task>>() ), Times.AtMostOnce() );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void StreamParserCanBeCreated()
		{
			// Arrange
			var userStream = new Mock<IStreamingConnection>();

			// Act
			var parser = StreamParser.Create( userStream.Object );

			// Assert
			Assert.IsNotNull( parser );
		}
	}
}