using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Twice.Models.Twitter
{
	internal interface ITwitterContext : IDisposable
	{
		Task<User> CreateBlockAsync( ulong userID, string screenName, bool skipStatus );

		Task<Status> CreateFavoriteAsync( ulong statusID );

		Task<Status> DeleteTweetAsync( ulong statusID );

		Task<Status> DestroyFavoriteAsync( ulong statusID );

		Task<Status> RetweetAsync( ulong statusID );

		Task<Status> TweetAsync( string text, IEnumerable<ulong> medias );

		Task<Media> UploadMediaAsync( byte[] mediaData, string mediaType, IEnumerable<ulong> additionalOwners );

		IAuthorizer Authorizer { get; }
		ITwitterFriendshipRepository Friendships { get; }
		ITwitterStatusRepository Statuses { get; }
		ITwitterQueryable<LinqToTwitter.Streaming> Streaming { get; }
		ITwitterUserRepository Users { get; }
	}
}