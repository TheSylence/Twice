using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Cache
{
	internal interface ICache : IDisposable
	{
		Task AddHashtag( string hashTag );

		Task AddStatus( Status status );

		Task AddStatuses( IList<Status> statuses );

		Task AddUser( UserCacheEntry user );

		Task<IEnumerable<string>> GetKnownHashtags();

		Task<IEnumerable<UserCacheEntry>> GetKnownUsers();

		Task<Status> GetStatus( ulong id );

		Task<List<Status>> GetStatusesForColumn( Guid columnId );

		Task MapStatusesToColumn( IList<Status> statuses, Guid columnId );

		Task<LinqToTwitter.Configuration> ReadTwitterConfig();

		Task SaveTwitterConfig( LinqToTwitter.Configuration cfg );
	}
}