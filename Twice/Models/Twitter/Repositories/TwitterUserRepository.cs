using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Twice.Models.Cache;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	internal class TwitterUserRepository : TwitterRepositoryBase, ITwitterUserRepository
	{
		public TwitterUserRepository( TwitterContext context, ICache cache )
			: base( context, cache )
		{
		}

		public Task<List<User>> LookupUsers( string userList )
		{
			return
				Queryable.Where( s => s.Type == UserType.Lookup && s.UserIdList == userList && s.IncludeEntities == false )
					.ToListAsync();
		}

		public Task<List<User>> Search( string query )
		{
			return Queryable.Where( s => s.Type == UserType.Search && s.Query == query ).ToListAsync();
		}

		public Task<User> ShowUser( ulong userId, bool includeEntities )
		{
			return
				Queryable.Where( s => s.UserID == userId && s.IncludeEntities == includeEntities && s.Type == UserType.Show )
					.SingleOrDefaultAsync();
		}

		public TwitterQueryable<User> Queryable => Context.User;
	}
}