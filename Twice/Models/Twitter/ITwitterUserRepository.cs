using LinqToTwitter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Twice.Models.Twitter
{
	internal interface ITwitterUserRepository
	{
		Task<List<User>> LookupUsers( string userList );

		Task<User> ShowUser( ulong userId, bool includeEntities );

		ITwitterQueryable<User> Queryable { get; }
	}
}