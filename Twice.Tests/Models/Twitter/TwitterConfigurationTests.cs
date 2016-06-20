using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Twice.Models.Cache;
using Twice.Models.Twitter;

namespace Twice.Tests.Models.Twitter
{
	[TestClass, ExcludeFromCodeCoverage]
	public class TwitterConfigurationTests
	{
		[TestMethod, TestCategory( "Models.Twitter" )]
		public async Task CachedConfigIsUsed()
		{
			// Arrange
			var config = new LinqToTwitter.Configuration();
			var cache = new Mock<ICache>();
			cache.Setup( c => c.ReadTwitterConfig() ).Returns( Task.FromResult( config ) );

			var contextList = new Mock<ITwitterContextList>( MockBehavior.Strict );

			var twitter = new TwitterConfiguration( cache.Object, contextList.Object );

			// Act
			await twitter.QueryConfig();

			// Assert
			cache.Verify( c => c.ReadTwitterConfig(), Times.Once() );
		}

		[TestMethod, TestCategory( "Models.Twitter" )]
		public async Task TwitterApiIsQueriedToLoadConfig()
		{
			// Arrange
			var cache = new Mock<ICache>();
			cache.Setup( c => c.SaveTwitterConfig( It.IsAny<LinqToTwitter.Configuration>() ) ).Returns( Task.CompletedTask );

			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.GetConfig() ).Returns( Task.FromResult( new LinqToTwitter.Configuration() ) );

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] { context.Object } );

			var twitter = new TwitterConfiguration( cache.Object, contextList.Object );

			// Act
			await twitter.QueryConfig();

			// Assert
			context.Verify( c => c.Twitter.GetConfig(), Times.Once() );
			cache.Verify( c => c.SaveTwitterConfig( It.IsAny<LinqToTwitter.Configuration>() ), Times.Once() );
		}
	}
}