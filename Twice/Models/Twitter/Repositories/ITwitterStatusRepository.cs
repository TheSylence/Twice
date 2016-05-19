using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterStatusRepository
	{
		Task<List<Status>> GetUserTweets( ulong userId, ulong since = 0, ulong max = 0 );

		Task<List<Status>> Filter( params Expression<Func<Status, bool>>[] filterExpressions );
		Task<Status> GetTweet( ulong statusId, bool includeEntities );
	}
}