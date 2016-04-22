using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	internal class TwitterFriendshipRepository : TwitterRepositoryBase, ITwitterFriendshipRepository
	{
		public TwitterFriendshipRepository( TwitterContext context )
			: base( context )
		{
		}

		public Task<Friendship> GetFriendshipWith( ulong thisUserId, ulong otherUserId )
		{
			return Queryable.Where( f => f.Type == FriendshipType.Show && f.TargetUserID == otherUserId &&
										f.SourceUserID == thisUserId ).SingleOrDefaultAsync();
		}

		public Task<List<User>> ListFollowers( ulong userId, int maxCount, bool skipStatus )
		{
			return Queryable.Where( f => f.Type == FriendshipType.FollowersList && f.UserID == userId.ToString()
										&& f.Count == maxCount && f.SkipStatus == skipStatus )
				.SelectMany( f => f.Users ).ToListAsync();
		}

		public Task<List<User>> ListFriends( ulong userId, int maxCount = 200, bool skipStatus = true )
		{
			return Queryable.Where( f => f.Type == FriendshipType.FriendsList && f.UserID == userId.ToString()
										&& f.Count == maxCount && f.SkipStatus == skipStatus )
				.SelectMany( f => f.Users ).ToListAsync();
		}

		public TwitterQueryable<Friendship> Queryable => Context.Friendship;
	}
}