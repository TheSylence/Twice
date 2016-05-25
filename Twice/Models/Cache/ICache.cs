using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Cache
{
	internal interface ICache : IDisposable
	{
		Task AddHashtags( IList<string> hashTags );

		Task AddStatuses( IList<Status> statuses );

		Task AddUsers( IList<UserCacheEntry> users );

		Task<IEnumerable<string>> GetKnownHashtags();

		Task<IEnumerable<UserCacheEntry>> GetKnownUsers();

		Task<Status> GetStatus( ulong id );

		Task<List<Status>> GetStatusesForColumn( Guid columnId );

		Task<List<Status>> GetStatusesForUser( ulong userId );

		Task<User> GetUser( ulong userId );

		Task MapStatusesToColumn( IList<Status> statuses, Guid columnId );

		Task<LinqToTwitter.Configuration> ReadTwitterConfig();

		Task RemoveStatus( ulong id );

		Task SaveTwitterConfig( LinqToTwitter.Configuration cfg );
	}
}