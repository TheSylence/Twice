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

		Task<LinqToTwitter.Configuration> GetConfig();

		Task LogCurrentRateLimits();

		Task ReportAsSpam( ulong userId );

		Task<Status> RetweetAsync( ulong statusId );

		Task<DirectMessage> SendMessage( string recipient, string message );

		Task<Status> TweetAsync( string text, IEnumerable<ulong> medias, ulong inReplyTo = 0 );

		Task<LinqToTwitter.Media> UploadMediaAsync( byte[] mediaData, string mediaType, IEnumerable<ulong> additionalOwners );

		Task<bool> VerifyCredentials();

		IAuthorizer Authorizer { get; }
		ITwitterFavoritesRepository Favorites { get; }
		ITwitterFriendshipRepository Friendships { get; }
		ITwitterMessageRepository Messages { get; }
		ITwitterSearchRepository Search { get; }
		ITwitterStatusRepository Statuses { get; }
		ITwitterStreamingRepository Streaming { get; }
		ITwitterUserRepository Users { get; }
	}
}