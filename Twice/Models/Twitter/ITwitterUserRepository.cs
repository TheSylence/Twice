using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Twitter
{
	internal interface ITwitterUserRepository
	{
		Task<List<User>> LookupUsers( string userList );

		ITwitterQueryable<User> Queryable { get; }
	}
}