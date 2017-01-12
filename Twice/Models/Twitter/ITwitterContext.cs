using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twice.Models.Twitter.Repositories;

namespace Twice.Models.Twitter
{
	internal interface ITwitterContext : IDisposable
	{
		Task<User> CreateBlockAsync( ulong userId, string screenName, bool skipStatus );

		string GetAuthorizationString( string requestUrl, string method = "GET" );

		Task<LinqToTwitter.Configuration> GetConfig();

		Task LogCurrentRateLimits();

		Task ReportAsSpam( ulong userId );

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