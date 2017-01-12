using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Twice.Models.Scheduling;
using Twice.Models.Scheduling.Processors;
using Twice.Models.Twitter;

namespace Twice.Tests.Models.Scheduling.Processors
{
	[TestClass, ExcludeFromCodeCoverage]
	public class CreateStatusProcessorTests
	{
		[TestMethod, TestCategory( "Models.Scheduling.Processors" )]
		public async Task ImagesAreUploaded()
		{
			try
			{
				// Arrange
				File.WriteAllBytes( "file1.png", new byte[] { 1, 2, 3, 4 } );
				File.WriteAllBytes( "file2.bmp", new byte[] { 5, 6, 7, 8 } );

				var ctx1 = new Mock<IContextEntry>();
				ctx1.Setup( c => c.UserId ).Returns( 1 );
				ctx1.Setup( c => c.Twitter.UploadMediaAsync( new byte[] { 1, 2, 3, 4 }, "image/png", new[] { 2ul } ) )
					.Returns( Task.FromResult( new LinqToTwitter.Media { MediaID = 888 } ) )
					.Verifiable();
				ctx1.Setup( c => c.Twitter.UploadMediaAsync( new byte[] { 5, 6, 7, 8 }, "image/bmp", new[] { 2ul } ) )
					.Returns( Task.FromResult( new LinqToTwitter.Media { MediaID = 999 } ) )
					.Verifiable();

				Func<IEnumerable<ulong>, bool> mediaCheck = ids => ids.Contains( 888ul ) && ids.Contains( 999ul );
				ctx1.Setup( c => c.Twitter.Statuses.TweetAsync( It.IsAny<string>(), It.Is<IEnumerable<ulong>>( ids => mediaCheck( ids ) ), It.IsAny<ulong>() ) )
					.Returns( Task.FromResult( DummyGenerator.CreateDummyStatus() ) )
					.Verifiable();

				var ctx2 = new Mock<IContextEntry>();
				ctx2.Setup( c => c.UserId ).Returns( 2 );
				ctx2.Setup( c => c.Twitter.Statuses.TweetAsync( It.IsAny<string>(), It.Is<IEnumerable<ulong>>( ids => mediaCheck( ids ) ), It.IsAny<ulong>() ) )
					.Returns( Task.FromResult( DummyGenerator.CreateDummyStatus() ) )
					.Verifiable();

				var contextList = new Mock<ITwitterContextList>();
				contextList.Setup( c => c.Contexts ).Returns( new[] { ctx1.Object, ctx2.Object } );

				var config = new Mock<ITwitterConfiguration>();
				config.SetupGet( c => c.MaxImageSize ).Returns( int.MaxValue );

				var proc = new CreateStatusProcessor( contextList.Object, config.Object );

				var job = new SchedulerJob
				{
					FilesToAttach = new List<string> { "file1.png", "file2.bmp" },
					AccountIds = new List<ulong> { 1, 2 }
				};

				// Act
				await proc.Process( job );

				// Assert
				ctx1.Verify( c => c.Twitter.UploadMediaAsync( new byte[] { 1, 2, 3, 4 }, "image/png", new[] { 2ul } ), Times.Once() );
				ctx1.Verify( c => c.Twitter.UploadMediaAsync( new byte[] { 5, 6, 7, 8 }, "image/bmp", new[] { 2ul } ), Times.Once() );

				ctx1.Verify( c => c.Twitter.Statuses.TweetAsync( It.IsAny<string>(), It.Is<IEnumerable<ulong>>( ids => mediaCheck( ids ) ), It.IsAny<ulong>() ),
					Times.Once() );
				ctx2.Verify( c => c.Twitter.Statuses.TweetAsync( It.IsAny<string>(), It.Is<IEnumerable<ulong>>( ids => mediaCheck( ids ) ), It.IsAny<ulong>() ),
					Times.Once() );
			}
			finally
			{
				File.Delete( "file1.png" );
				File.Delete( "file2.bmp" );
			}
		}

		[TestMethod, TestCategory( "Models.Scheduling.Processors" )]
		public async Task TooBigImageIsNotUploaded()
		{
			try
			{
				// Arrange
				File.WriteAllBytes( "file3.png", new byte[] { 1 } );

				var ctx1 = new Mock<IContextEntry>();
				ctx1.Setup( c => c.UserId ).Returns( 1 );
				ctx1.Setup( c => c.Twitter.UploadMediaAsync( It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<IEnumerable<ulong>>() ) )
					.Returns( Task.FromResult( new LinqToTwitter.Media() ) )
					.Verifiable();

				var config = new Mock<ITwitterConfiguration>();
				config.SetupGet( c => c.MaxImageSize ).Returns( 0 );

				var contextList = new Mock<ITwitterContextList>();
				contextList.Setup( c => c.Contexts ).Returns( new[] { ctx1.Object } );

				var proc = new CreateStatusProcessor( contextList.Object, config.Object );

				var job = new SchedulerJob
				{
					FilesToAttach = new List<string> { "file3.png" }
				};

				// Act
				await proc.Process( job );

				// Assert
				ctx1.Verify( c => c.Twitter.UploadMediaAsync( It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<IEnumerable<ulong>>() ), Times.Never() );
			}
			finally
			{
				File.Delete( "file3.png" );
			}
		}

		[TestMethod, TestCategory( "Models.Scheduling.Processors" )]
		public async Task TweetIsSend()
		{
			// Arrange
			var ctx1 = new Mock<IContextEntry>();
			ctx1.Setup( c => c.UserId ).Returns( 1 );
			ctx1.Setup( c => c.Twitter.Statuses.TweetAsync( "hello world", It.IsAny<IEnumerable<ulong>>(), 111 ) )
				.Returns( Task.FromResult( DummyGenerator.CreateDummyStatus() ) )
				.Verifiable();

			var ctx2 = new Mock<IContextEntry>();
			ctx2.Setup( c => c.UserId ).Returns( 2 );
			ctx2.Setup( c => c.Twitter.Statuses.TweetAsync( "hello world", It.IsAny<IEnumerable<ulong>>(), 111 ) )
				.Returns( Task.FromResult( DummyGenerator.CreateDummyStatus() ) )
				.Verifiable();

			var contextList = new Mock<ITwitterContextList>();
			contextList.Setup( c => c.Contexts ).Returns( new[] { ctx1.Object, ctx2.Object } );

			var job = new SchedulerJob
			{
				AccountIds = new List<ulong> { 1, 2 },
				Text = "hello world",
				InReplyToStatus = 111
			};

			var config = new Mock<ITwitterConfiguration>();
			var proc = new CreateStatusProcessor( contextList.Object, config.Object );

			// Act
			await proc.Process( job );

			// Assert
			ctx1.Verify( c => c.Twitter.Statuses.TweetAsync( "hello world", It.IsAny<IEnumerable<ulong>>(), 111 ), Times.Once() );
			ctx2.Verify( c => c.Twitter.Statuses.TweetAsync( "hello world", It.IsAny<IEnumerable<ulong>>(), 111 ), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Scheduling.Processors" )]
		public async Task UnknownAccountIsNotUsedForTweeting()
		{
			// Arrange
			var job = new SchedulerJob
			{
				AccountIds = new List<ulong> { 3 },
				Text = "test"
			};

			var ctx1 = new Mock<IContextEntry>();
			ctx1.Setup( c => c.UserId ).Returns( 1 );
			ctx1.Setup( c => c.Twitter.Statuses.TweetAsync( "test", It.IsAny<IEnumerable<ulong>>(), 0 ) )
				.Returns( Task.FromResult( DummyGenerator.CreateDummyStatus() ) )
				.Verifiable();

			var contextList = new Mock<ITwitterContextList>();
			contextList.Setup( c => c.Contexts ).Returns( new[] { ctx1.Object } );

			var config = new Mock<ITwitterConfiguration>();
			var proc = new CreateStatusProcessor( contextList.Object, config.Object );

			// Act
			await proc.Process( job );

			// Assert
			ctx1.Verify( c => c.Twitter.Statuses.TweetAsync( "test", It.IsAny<IEnumerable<ulong>>(), 0 ), Times.Never() );
		}
	}
}