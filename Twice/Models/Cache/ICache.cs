using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Twice.Models.Cache
{
	internal interface ICache : IDisposable
	{
		Task AddHashtag( string hashTag );

		Task AddUser( UserCacheEntry user );

		Task<IEnumerable<UserCacheEntry>> GetKnownUsers();

		Task<IEnumerable<string>> GetKnownHashtags();

		Task<LinqToTwitter.Configuration> ReadTwitterConfig();

		Task SaveTwitterConfig( LinqToTwitter.Configuration cfg );
	}
}