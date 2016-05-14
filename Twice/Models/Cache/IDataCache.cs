using System.Collections.Generic;
using System.Threading.Tasks;

namespace Twice.Models.Cache
{
	internal interface IDataCache
	{
		Task AddHashtag( string hashtag );

		Task AddUser( ulong id, string name );

		Task<IEnumerable<string>> GetKnownHashtags();

		Task<IEnumerable<ulong>> GetKnownUserIds();

		Task<IEnumerable<UserCacheEntry>> GetKnownUsers();

		Task<LinqToTwitter.Configuration> ReadTwitterConfig();

		Task SaveTwitterConfig( LinqToTwitter.Configuration cfg );
	}
}