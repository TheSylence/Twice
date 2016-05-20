using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SimpleCacheSharp;
using Twice.Models.Cache;

namespace Twice.Tests.Models.Cache
{
	[TestClass, ExcludeFromCodeCoverage]
	public class DataCacheTests
	{
		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task HashtagCanBeCached()
		{
			// Arrange
			var backend = new Mock<ICache>();
			var cache = new DataCache( backend.Object );

			// Act
			await cache.AddHashtag( "#HashTagTest" );

			// Assert
			backend.Verify( b => b.Set( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task HashtagCanBeRetrieved()
		{
			// Arrange
			var keys = new[] {"HASHTAG:Test", "HASHTAG:Abc"};
			var values = new Dictionary<string, string>
			{
				{"HASHTAG:Test", JsonConvert.SerializeObject( new HashtagCacheEntry( "Test" ) )},
				{"HASHTAG:Abc", JsonConvert.SerializeObject( new HashtagCacheEntry( "Abc" ) )}
			};

			var backend = new Mock<ICache>();
			backend.Setup( b => b.GetKeys() ).Returns( Task.FromResult( keys.ToList() ) );
			backend.Setup( b => b.Get( "HASHTAG:Test" ) ).Returns( Task.FromResult( values["HASHTAG:Test"] ) );
			backend.Setup( b => b.Get( "HASHTAG:Abc" ) ).Returns( Task.FromResult( values["HASHTAG:Abc"] ) );
			var cache = new DataCache( backend.Object );

			// Act
			var tags = ( await cache.GetKnownHashtags() ).ToArray();

			// Assert
			Assert.AreEqual( 2, tags.Length );
			CollectionAssert.Contains( tags, "Test" );
			CollectionAssert.Contains( tags, "Abc" );
		}

		[TestMethod, TestCategory( "Modeels.Cache" )]
		public async Task TwitterConfigCanBeRead()
		{
			// Arrange
			var backend = new Mock<ICache>();
			backend.Setup(
				c =>
					c.Get( DataCache.ConfigurationKey ) ).Returns(
						Task.FromResult( JsonConvert.SerializeObject( new LinqToTwitter.Configuration() ) ) );
			;
			var cache = new DataCache( backend.Object );

			// Act
			var cfg = await cache.ReadTwitterConfig();

			// Assert
			Assert.IsNotNull( cfg );
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task TwitterConfigCanBeStored()
		{
			// Arrange
			var backend = new Mock<ICache>();
			var cache = new DataCache( backend.Object );
			var cfg = new LinqToTwitter.Configuration();

			// Act
			await cache.SaveTwitterConfig( cfg );

			// Assert
			backend.Verify( b => b.Set( DataCache.ConfigurationKey, It.IsAny<string>(), It.IsAny<TimeSpan>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task UserCanBeAdded()
		{
			// Arrange
			var backend = new Mock<ICache>();
			var cache = new DataCache( backend.Object );

			// Act
			await cache.AddUser( 123, "Testi" );

			// Assert
			backend.Verify( b => b.Set( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task UserIdsCanBeRetrieved()
		{
			// Arrange
			var keys = new[] {"USER:123", "USER:111"};
			var values = new Dictionary<string, string>
			{
				{"USER:123", JsonConvert.SerializeObject( new UserCacheEntry( 123, "Test" ) )},
				{"USER:111", JsonConvert.SerializeObject( new UserCacheEntry( 111, "Abc" ) )}
			};

			var backend = new Mock<ICache>();
			backend.Setup( b => b.GetKeys() ).Returns( Task.FromResult( keys.ToList() ) );
			backend.Setup( b => b.Get( "USER:123" ) ).Returns( Task.FromResult( values["USER:123"] ) );
			backend.Setup( b => b.Get( "USER:111" ) ).Returns( Task.FromResult( values["USER:111"] ) );
			var cache = new DataCache( backend.Object );

			// Act
			var userIds = ( await cache.GetKnownUserIds() ).ToArray();

			// Assert
			Assert.AreEqual( 2, userIds.Length );
			CollectionAssert.Contains( userIds, 123ul );
			CollectionAssert.Contains( userIds, 111ul );
		}
	}
}