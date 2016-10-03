using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;
using Twice.Models.Twitter.Entities;

namespace Twice.Models.Cache
{
	internal interface ICache : IDisposable
	{
		Task AddHashtags( IList<string> hashTags );

		Task AddMessages( IList<MessageCacheEntry> messages );

		Task AddStatuses( IList<Status> statuses );

		Task AddUsers( IList<UserCacheEntry> users );

		Task Clear();

		Task<ulong> FindFriend( ulong friendId );

		Task<IEnumerable<string>> GetKnownHashtags();

		Task<IEnumerable<UserCacheEntry>> GetKnownUsers();

		Task<List<MessageCacheEntry>> GetMessages();

		Task<Status> GetStatus( ulong id );

		Task<List<Status>> GetStatusesForColumn( Guid columnId, int limit );

		Task<List<Status>> GetStatusesForUser( ulong userId );

		Task<UserEx> GetUser( ulong userId );

		Task<IEnumerable<ulong>> GetUserFriends( ulong userId );

		Task MapStatusesToColumn( IList<Status> statuses, Guid columnId );

		Task<LinqToTwitter.Configuration> ReadTwitterConfig();

		Task RemoveStatus( ulong id );

		Task SaveTwitterConfig( LinqToTwitter.Configuration cfg );

		Task SetUserFriends( ulong userId, IEnumerable<ulong> friendIds );
	}
}