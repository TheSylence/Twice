using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Anotar.NLog;
using Fody;
using LinqToTwitter;
using Twice.Models.Cache;
using Twice.Models.Twitter.Comparers;

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

		public async Task<List<ulong>> FindRetweeters( ulong statusId, int count )
		{
			var response = await Queryable.Where( s => s.Type == StatusType.Retweeters && s.ID == statusId )
				.SingleOrDefaultAsync();

			List<ulong> result;
			if( response?.Users != null )
			{
				result = response.Users;
			}
			else
			{
				result = new List<ulong>();
			}

			return result.Take( count ).ToList();
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
				var status = await Queryable.Where( s => s.Type == StatusType.Show && s.ID == statusId
														&& s.IncludeEntities == includeEntities ).FirstOrDefaultAsync();

				await Cache.AddStatuses( new[] {status} );
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
			Debug.Assert( since == 0 || max == 0 );

			IEnumerable<Status> cached = await Cache.GetStatusesForUser( userId );
			if( since != 0 )
			{
				var since1 = since;
				cached = cached.Where( c => c.GetStatusId() > since1 );
			}
			if( max != 0 )
			{
				var max1 = max;
				cached = cached.Where( c => c.GetStatusId() < max1 );
			}

			var cachedList = cached.ToList();
			var query = Queryable.Where( s => s.Type == StatusType.User && s.UserID == userId );

			if( since != 0 )
			{
				if( cachedList.Any() )
				{
					since = Math.Max( cachedList.Max( c => c.GetStatusId() ), since );
				}
				query = query.Where( s => s.SinceID == since );
			}
			if( max != 0 )
			{
				if( cachedList.Any() )
				{
					max = Math.Min( cachedList.Min( c => c.GetStatusId() ), max );
				}
				query = query.Where( s => s.MaxID == max );
			}

			var statusList = await query.ToListAsync();
			var newStatuses = statusList.Except( cachedList, TwitterComparers.StatusComparer ).ToList();
			await Cache.AddStatuses( newStatuses );

			cachedList.AddRange( newStatuses );
			return cachedList;
		}

		public TwitterQueryable<Status> Queryable => Context.Status;
	}
}