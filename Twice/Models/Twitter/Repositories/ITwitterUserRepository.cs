using LinqToTwitter;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twice.Models.Twitter.Entities;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterUserRepository
	{
		Task FollowUser( ulong userId );

		Task<List<UserEx>> LookupUsers( IEnumerable<ulong> userIDs );

		Task<List<UserEx>> LookupUsers( string userList );

		Task<UserEx> ShowUser( ulong userId, bool includeEntities );

		Task UnfollowUser( ulong userId );
	}
}