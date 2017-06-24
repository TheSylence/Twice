using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Twice.Models.Cache;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	[ConfigureAwait( false )]
	internal class TwitterFavoritesRepository : TwitterRepositoryBase, ITwitterFavoritesRepository
	{
		public TwitterFavoritesRepository( TwitterContext context, ICache cache ) : base( context, cache )
		{
		}

		public Task<Status> CreateFavoriteAsync( ulong statusId )
		{
			return Context.CreateFavoriteAsync( statusId );
		}

		public Task<Status> DestroyFavoriteAsync( ulong statusId )
		{
			return Context.DestroyFavoriteAsync( statusId );
		}

		public async Task<List<Favorites>> List( ulong userId, int count, params Expression<Func<Favorites, bool>>[] filterExpressions )
		{
			IQueryable<Favorites> query = Queryable.Where( f => f.UserID == userId && f.Type == FavoritesType.Favorites && f.Count == count );

			foreach( var filter in filterExpressions )
			{
				query = query.Where( filter );
			}

			return await query.ToListAsync();
		}

		public Task<List<Favorites>> List( ulong userId, params Expression<Func<Favorites, bool>>[] filterExpressions )
		{
			return List( userId, 200, filterExpressions );
		}

		private TwitterQueryable<Favorites> Queryable => Context.Favorites;
	}
}