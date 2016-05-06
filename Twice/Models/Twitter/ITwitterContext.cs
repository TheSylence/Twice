using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;
using Twice.Models.Twitter.Repositories;

namespace Twice.Models.Twitter
{
	internal interface ITwitterContext : IDisposable
	{
		Task<User> CreateBlockAsync( ulong userId, string screenName, bool skipStatus );

		Task<Status> CreateFavoriteAsync( ulong statusId );

		Task<Status> DeleteTweetAsync( ulong statusId );

		Task<Status> DestroyFavoriteAsync( ulong statusId );

		Task<Status> RetweetAsync( ulong statusId );

		Task<Status> TweetAsync( string text, IEnumerable<ulong> medias );

		Task<Media> UploadMediaAsync( byte[] mediaData, string mediaType, IEnumerable<ulong> additionalOwners );

		IAuthorizer Authorizer { get; }
		ITwitterFriendshipRepository Friendships { get; }
		ITwitterStatusRepository Statuses { get; }
		ITwitterStreamingRepository Streaming { get; }
		ITwitterUserRepository Users { get; }
	}
}