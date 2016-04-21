using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Twice.Models.Twitter
{
	internal interface ITwitterStatusRepository
	{
		Task<List<Status>> Filter( params Expression<Func<Status, bool>>[] filterExpressions );

		ITwitterQueryable<Status> Queryable { get; }
	}
}