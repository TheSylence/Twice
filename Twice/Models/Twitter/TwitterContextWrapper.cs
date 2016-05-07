using LinqToTwitter;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Twice.Models.Twitter.Repositories;

namespace Twice.Models.Twitter
{
	[ExcludeFromCodeCoverage]
	internal class TwitterContextWrapper : ITwitterContext
	{
		public TwitterContextWrapper( TwitterContext context )
		{
			Context = context;

			Users = new TwitterUserRepository( context );
			Statuses = new TwitterStatusRepository( context );
			Friendships = new TwitterFriendshipRepository( context );
			Streaming = new TwitterStreamingRepository( context );
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

		public Task<Status> RetweetAsync( ulong statusId )
		{
			return Context.RetweetAsync( statusId );
		}

		public Task<Status> TweetAsync( string text, IEnumerable<ulong> medias )
		{
			return Context.TweetAsync( text, medias );
		}

		public Task<Media> UploadMediaAsync( byte[] mediaData, string mediaType, IEnumerable<ulong> additionalOwners )
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