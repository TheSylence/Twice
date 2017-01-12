using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Entities;
using Twice.Models.Twitter.Repositories;
using Twice.Models.Twitter.Streaming;

namespace Twice.Tests.Models.Twitter.Streaming
{
	[TestClass, ExcludeFromCodeCoverage]
	public class StreamingRepositoryTests
	{
		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void ParsersAreReused()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();

			var parser = new Mock<IStreamParser>();
			parser.Setup( p => p.Dispose() ).Verifiable();

			var repo = new TestStreamingRepository( contextList.Object, null );
			repo.InjectStream( 123, parser.Object );

			// Act
			var second = repo.GetParser( new ColumnDefinition( ColumnType.User ) { SourceAccounts = new ulong[] { 123 } } );

			// Assert
			Assert.AreSame( parser.Object, second );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void ReceivedFriendsAreStoredAsFriendsInCache()
		{
			// Arrange
			var userList = new List<UserEx>
			{
				new UserEx {UserID = 123, IncludeEntities = false, Type = UserType.Lookup, UserIdList = "123,456,789"},
				new UserEx {UserID = 456, IncludeEntities = false, Type = UserType.Lookup, UserIdList = "123,456,789"},
				new UserEx {UserID = 789, IncludeEntities = false, Type = UserType.Lookup, UserIdList = "123,456,789"}
			};

			var contextList = new Mock<ITwitterContextList>();
			var twitterContext = new Mock<ITwitterContext>();
			var userRepo = new Mock<ITwitterUserRepository>();
			userRepo.Setup( t => t.LookupUsers( "123,456,789" ) ).Returns( Task.FromResult( userList ) );
			twitterContext.SetupGet( c => c.Users ).Returns( userRepo.Object );

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.Twitter ).Returns( twitterContext.Object );
			context.SetupGet( c => c.UserId ).Returns( 111 );

			var parser = new Mock<IStreamParser>();
			parser.SetupGet( c => c.AssociatedContext ).Returns( context.Object );

			var cache = new Mock<ICache>();
			cache.Setup( c => c.SetUserFriends( 111, It.IsAny<IEnumerable<ulong>>() ) ).Returns( Task.CompletedTask ).Verifiable();

			var repo = new TestStreamingRepository( contextList.Object, cache.Object );
			repo.InjectStream( 123, parser.Object );

			// Act
			parser.Raise( p => p.FriendsReceived += null, new FriendsStreamEventArgs( "{\"friends\":[123,456,789]}" ) );

			// Assert
			cache.Verify( c => c.SetUserFriends( 111, It.IsAny<IEnumerable<ulong>>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void ReceivingFriendsStoresThemInCache()
		{
			// Arrange
			var userList = new List<UserEx>
			{
				new UserEx {UserID = 123, IncludeEntities = false, Type = UserType.Lookup, UserIdList = "123,456,789"},
				new UserEx {UserID = 456, IncludeEntities = false, Type = UserType.Lookup, UserIdList = "123,456,789"},
				new UserEx {UserID = 789, IncludeEntities = false, Type = UserType.Lookup, UserIdList = "123,456,789"}
			};

			var contextList = new Mock<ITwitterContextList>();
			var twitterContext = new Mock<ITwitterContext>();
			var userRepo = new Mock<ITwitterUserRepository>();
			userRepo.Setup( t => t.LookupUsers( "123,456,789" ) ).Returns( Task.FromResult( userList ) );
			twitterContext.SetupGet( c => c.Users ).Returns( userRepo.Object );

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.Twitter ).Returns( twitterContext.Object );

			var parser = new Mock<IStreamParser>();
			parser.SetupGet( c => c.AssociatedContext ).Returns( context.Object );

			var cache = new Mock<ICache>();
			cache.Setup( c => c.AddUsers( It.IsAny<IList<UserCacheEntry>>() ) ).Returns( Task.CompletedTask );

			var repo = new TestStreamingRepository( contextList.Object, cache.Object );
			repo.InjectStream( 123, parser.Object );

			// Act
			parser.Raise( p => p.FriendsReceived += null, new FriendsStreamEventArgs( "{\"friends\":[123,456,789]}" ) );

			// Assert
			cache.Verify( c => c.AddUsers( It.IsAny<IList<UserCacheEntry>>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Twitter.Streaming" )]
		public void StreamsAreDisposedWhenRepositoryIs()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();

			var parser = new Mock<IStreamParser>();
			parser.Setup( p => p.Dispose() ).Verifiable();

			var repo = new TestStreamingRepository( contextList.Object, null );
			repo.InjectStream( 123, parser.Object );

			// Act
			repo.Dispose();

			// Assert
			parser.Verify( p => p.Dispose(), Times.Once() );
		}

		private class TestStreamingRepository : StreamingRepository
		{
			public TestStreamingRepository( ITwitterContextList contextList, ICache cache )
				: base( contextList, cache )
			{
			}

			public void InjectStream( ulong id, IStreamParser parser )
			{
				AddParser( parser, new ParserKey( id, StreamingType.User ) );
			}
		}
	}
}