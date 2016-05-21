using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

		Task<LinqToTwitter.Configuration> ReadTwitterConfig();

		Task SaveTwitterConfig( LinqToTwitter.Configuration cfg );
	}
}