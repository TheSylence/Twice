using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Anotar.NLog;
using LinqToTwitter;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	internal class TwitterStatusRepository : TwitterRepositoryBase, ITwitterStatusRepository
	{
		public TwitterStatusRepository( TwitterContext context )
			: base( context )
		{
		}

		public Task<List<Status>> Filter( params Expression<Func<Status, bool>>[] filterExpressions )
		{
			IQueryable<Status> query = Queryable;

			foreach( var filter in filterExpressions )
			{
				query = query.Where( filter );
			}

			return query.ToListAsync();
		}

		public async Task<Status> GetTweet( ulong statusId, bool includeEntities )
		{
			try
			{
				var status = await Queryable.Where( s => s.Type == StatusType.Show && s.StatusID == statusId
														&& s.IncludeEntities == includeEntities ).FirstOrDefaultAsync();

				return status;
			}
			catch( TwitterQueryException ex )
			{
				LogTo.ErrorException( $"Failed to retrieve status with id {statusId}", ex );
				return null;
			}
		}

		public Task<List<Status>> GetUserTweets( ulong userId, ulong since = 0, ulong max = 0 )
		{
			var query = Queryable.Where( s => s.Type == StatusType.User && s.UserID == userId );
			if( since != 0 )
			{
				query = query.Where( s => s.SinceID == since );
			}
			if( max != 0 )
			{
				query = query.Where( s => s.MaxID == max );
			}

			return query.ToListAsync();
		}

		public TwitterQueryable<Status> Queryable => Context.Status;
	}
}