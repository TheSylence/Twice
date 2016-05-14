using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;

namespace Twice.Models.Cache
{
	internal class DataCache : IDataCache
	{
		public DataCache( ISecureBlobCache secure, IBlobCache data )
		{
			Secure = secure;
			Cache = data;
		}

		public async Task AddHashtag( string hashtag )
		{
			var data = new HashtagCacheEntry( hashtag );
			await Cache.InsertObject( data.GetKey(), data, DateTimeOffset.Now.Add( Constants.Cache.HashtagExpiration ) );
		}

		public async Task AddUser( ulong id, string name )
		{
			UserCacheEntry data = new UserCacheEntry( id, name );

			await Cache.InsertObject( data.GetKey(), data, DateTimeOffset.Now.Add( Constants.Cache.UserInfoExpiration ) );
		}

		public async Task<IEnumerable<string>> GetKnownHashtags()
		{
			var result = await Cache.GetAllObjects<HashtagCacheEntry>();

			return result.Select( r => r.Hashtag );
		}

		public async Task<IEnumerable<ulong>> GetKnownUserIds()
		{
			var result = await Cache.GetAllObjects<UserCacheEntry>();

			return result.Select( u => u.Id );
		}

		public async Task<IEnumerable<UserCacheEntry>> GetKnownUsers()
		{
			return await Cache.GetAllObjects<UserCacheEntry>();
		}

		public async Task<LinqToTwitter.Configuration> ReadTwitterConfig()
		{
			return await Cache.GetObject<LinqToTwitter.Configuration>( ConfigurationKey )
				.Catch( Observable.Return<LinqToTwitter.Configuration>( null ) );
		}

		public async Task SaveTwitterConfig( LinqToTwitter.Configuration cfg )
		{
			await Cache.InsertObject( ConfigurationKey, cfg, TimeSpan.FromDays( 1 ) );
		}

		private const string ConfigurationKey = "twitter.help.configuration";

		public IBlobCache Cache { get; }
		public ISecureBlobCache Secure { get; }
	}
}