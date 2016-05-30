using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterUserRepository
	{
		Task FollowUser( ulong userId );

		Task<List<User>> LookupUsers( IEnumerable<ulong> userIDs );

		Task<List<User>> LookupUsers( string userList );

		Task<List<User>> Search( string query );

		Task<User> ShowUser( ulong userId, bool includeEntities );

		Task UnfollowUser( ulong userId );
	}
}