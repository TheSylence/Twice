using Anotar.NLog;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fody;
using Twice.Models.Cache;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	[ConfigureAwait( false )]
	internal class TwitterStatusRepository : TwitterRepositoryBase, ITwitterStatusRepository
	{
		public TwitterStatusRepository( TwitterContext context, ICache cache )
			: base( context, cache )
		{
		}

		public async Task<List<Status>> Filter( params Expression<Func<Status, bool>>[] filterExpressions )
		{
			IQueryable<Status> query = Queryable;

			foreach( var filter in filterExpressions )
			{
				query = query.Where( filter );
			}
			

			var statusList = await query.ToListAsync();
			await Cache.AddStatuses( statusList );
			return statusList;
		}

		public async Task<Status> GetTweet( ulong statusId, bool includeEntities )
		{
			var cached = await Cache.GetStatus( statusId );
			if( cached != null )
			{
				return cached;
			}

			try
			{
				var status = await Queryable.Where( s => s.Type == StatusType.Show && s.StatusID == statusId
														&& s.IncludeEntities == includeEntities ).FirstOrDefaultAsync();

				await Cache.AddStatuses( new[] { status} );
				return status;
			}
			catch( TwitterQueryException ex )
			{
				LogTo.ErrorException( $"Failed to retrieve status with id {statusId}", ex );
				return null;
			}
		}

		public async Task<List<Status>> GetUserTweets( ulong userId, ulong since = 0, ulong max = 0 )
		{
			var cached = await Cache.GetStatusesForUser( userId );
			if( cached.Any() )
			{
				since = cached.Max( c => c.GetStatusId() );
			}

			var query = Queryable.Where( s => s.Type == StatusType.User && s.UserID == userId );
			if( since != 0 )
			{
				query = query.Where( s => s.SinceID == since );
			}
			if( max != 0 )
			{
				query = query.Where( s => s.MaxID == max );
			}

			var statusList = await query.ToListAsync();
			await Cache.AddStatuses( statusList );

			cached.AddRange( statusList );
			return cached;
		}

		public TwitterQueryable<Status> Queryable => Context.Status;
	}
}