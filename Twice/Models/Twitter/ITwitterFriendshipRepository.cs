using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Twitter
{
	internal interface ITwitterFriendshipRepository
	{
		Task<Friendship> GetFriendshipWith( ulong thisUserId, ulong otherUserId );

		Task<List<User>> ListFollowers( ulong userId, int maxCount = 200, bool skipStatus = true );
		Task<List<User>> ListFriends( ulong userId, int maxCount = 200, bool skipStatus = true );

		ITwitterQueryable<Friendship> Queryable { get; }
	}
}