using LinqToTwitter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterFriendshipRepository
	{
		Task<Friendship> GetFriendshipWith( ulong thisUserId, ulong otherUserId );

		Task<List<User>> ListFollowers( ulong userId, int maxCount = 200, bool skipStatus = true );

		Task<List<User>> ListFriends( ulong userId, int maxCount = 200, bool skipStatus = true );
	}
}