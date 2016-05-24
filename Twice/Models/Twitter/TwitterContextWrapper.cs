using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Twice.Models.Cache;
using Twice.Models.Twitter.Repositories;

namespace Twice.Models.Twitter
{
	[ExcludeFromCodeCoverage]
	[ConfigureAwait( false )]
	internal class TwitterContextWrapper : ITwitterContext
	{
		public TwitterContextWrapper( TwitterContext context, ICache cache )
		{
			Context = context;

			Users = new TwitterUserRepository( context, cache );
			Statuses = new TwitterStatusRepository( context, cache );
			Friendships = new TwitterFriendshipRepository( context, cache );
			Streaming = new TwitterStreamingRepository( context, cache );
		}

		public Task<User> CreateBlockAsync( ulong userId, string screenName, bool skipStatus )
		{
			return Context.CreateBlockAsync( userId, screenName, skipStatus );
		}

		public Task<Status> CreateFavoriteAsync( ulong statusId )
		{
			return Context.CreateFavoriteAsync( statusId );
		}

		public Task<Status> DeleteTweetAsync( ulong statusId )
		{
			return Context.DeleteTweetAsync( statusId );
		}

		public Task<Status> DestroyFavoriteAsync( ulong statusId )
		{
			return Context.DestroyFavoriteAsync( statusId );
		}

		public void Dispose()
		{
			Context.Dispose();
		}

		public async Task<LinqToTwitter.Configuration> GetConfig()
		{
			var help = await Context.Help.Where( h => h.Type == HelpType.Configuration ).SingleOrDefaultAsync();

			return help.Configuration;
		}

		public Task<Status> RetweetAsync( ulong statusId )
		{
			return Context.RetweetAsync( statusId );
		}

		public Task<Status> TweetAsync( string text, IEnumerable<ulong> medias )
		{
			return Context.TweetAsync( text, medias );
		}

		public Task<LinqToTwitter.Media> UploadMediaAsync( byte[] mediaData, string mediaType, IEnumerable<ulong> additionalOwners )
		{
			return Context.UploadMediaAsync( mediaData, mediaType, additionalOwners );
		}

		public IAuthorizer Authorizer => Context.Authorizer;
		public ITwitterFriendshipRepository Friendships { get; }
		public ITwitterStatusRepository Statuses { get; }
		public ITwitterStreamingRepository Streaming { get; }
		public ITwitterUserRepository Users { get; }
		private readonly TwitterContext Context;
	}
}