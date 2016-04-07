using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Twitter
{
	internal class TwitterContextWrapper : ITwitterContext
	{
		public TwitterContextWrapper( TwitterContext context )
		{
			Context = context;

			Streaming = new TwitterQueryableWrapper<LinqToTwitter.Streaming>( context.Streaming );
			Status = new TwitterQueryableWrapper<Status>( context.Status );
			User = new TwitterQueryableWrapper<User>( context.User );
			Friendship = new TwitterQueryableWrapper<Friendship>( context.Friendship );
		}

		public Task<User> CreateBlockAsync( ulong userID, string screenName, bool skipStatus )
		{
			return Context.CreateBlockAsync( userID, screenName, skipStatus );
		}

		public Task<Status> CreateFavoriteAsync( ulong statusID )
		{
			return Context.CreateFavoriteAsync( statusID );
		}

		public Task<Status> DeleteTweetAsync( ulong statusID )
		{
			return Context.DeleteTweetAsync( statusID );
		}

		public Task<Status> DestroyFavoriteAsync( ulong statusID )
		{
			return Context.DestroyFavoriteAsync( statusID );
		}

		public void Dispose()
		{
			Context.Dispose();
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
		public ITwitterQueryable<Friendship> Friendship { get; }
		public ITwitterQueryable<Status> Status { get; }
		public ITwitterQueryable<LinqToTwitter.Streaming> Streaming { get; }
		public ITwitterQueryable<User> User { get; }
		public Task<Status> RetweetAsync( ulong statusID )
		{
			return Context.RetweetAsync( statusID );
		}

		private readonly TwitterContext Context;
	}
}