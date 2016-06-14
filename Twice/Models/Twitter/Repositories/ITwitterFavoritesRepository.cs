using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterFavoritesRepository
	{
		Task<Status> CreateFavoriteAsync( ulong statusId );

		Task<Status> DestroyFavoriteAsync( ulong statusId );

		Task<List<Favorites>> List( ulong userId, params Expression<Func<Favorites, bool>>[] filterExpressions );

		Task<List<Favorites>> List( ulong userId, int count, params Expression<Func<Favorites, bool>>[] filterExpressions );
	}
}