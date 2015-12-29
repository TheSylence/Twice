using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;

namespace Twice.Models.Cache
{
	internal class UserCache : IUserCache
	{
		public UserCache( IBlobCache cache )
		{
			Cache = cache;
		}

		public async Task Add( ulong id, string name )
		{
			UserCacheEntry data = new UserCacheEntry( id, name );

			await Cache.InsertObject( data.GetKey(), data, DateTimeOffset.Now.Add( Constants.Cache.UserInfoExpiration ) );
		}

		public async Task<IEnumerable<ulong>> GetKnownUsers()
		{
			var result = await Cache.GetAllObjects<UserCacheEntry>();

			return result.Select( u => u.Id );
		}

		private readonly IBlobCache Cache;
	}
}