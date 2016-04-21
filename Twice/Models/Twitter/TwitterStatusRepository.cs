using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Twice.Models.Twitter
{
	[ExcludeFromCodeCoverage]
	internal class TwitterStatusRepository : TwitterRepositoryBase, ITwitterStatusRepository
	{
		public TwitterStatusRepository( TwitterContext context )
			: base( context )
		{
			Queryable = new TwitterQueryableWrapper<Status>( context.Status );
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

		public ITwitterQueryable<Status> Queryable { get; }
	}
}