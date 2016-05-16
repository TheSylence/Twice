using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleCache;

namespace Twice.Models.Cache
{
	internal class DataCache : IDataCache
	{
		public DataCache( ICache cache )
			: this( Constants.IO.CacheFileName )
		{
			Cache = cache;
		}

		public DataCache( string fileName )
		{
		}

		public async Task AddHashtag( string hashtag )
		{
			var data = new HashtagCacheEntry( hashtag );
			var key = data.GetKey();
			var value = JsonConvert.SerializeObject( data );

			await Cache.Set( key, value, Constants.Cache.HashtagExpiration );
		}

		public async Task AddUser( ulong id, string name )
		{
			UserCacheEntry data = new UserCacheEntry( id, name );
			var key = data.GetKey();
			var value = JsonConvert.SerializeObject( data );

			await Cache.Set( key, value, Constants.Cache.UserInfoExpiration );
		}

		public async Task<IEnumerable<string>> GetKnownHashtags()
		{
			var keys = ( await Cache.GetKeys() ).Where( k => k.StartsWith( "HASHTAG:", StringComparison.Ordinal ) );
			var result = new List<string>();

			foreach( var key in keys )
			{
				var json = await Cache.Get( key );
				var tag = JsonConvert.DeserializeObject<HashtagCacheEntry>( json );

				result.Add( tag.Hashtag );
			}

			return result;
		}

		public async Task<IEnumerable<ulong>> GetKnownUserIds()
		{
			var keys = ( await Cache.GetKeys() ).Where( k => k.StartsWith( "USER:", StringComparison.Ordinal ) );
			var result = new List<ulong>();

			foreach( var key in keys )
			{
				var json = await Cache.Get( key );
				var user = JsonConvert.DeserializeObject<UserCacheEntry>( json );

				result.Add( user.Id );
			}

			return result;
		}

		public async Task<IEnumerable<UserCacheEntry>> GetKnownUsers()
		{
			var keys = ( await Cache.GetKeys() ).Where( k => k.StartsWith( "USER:", StringComparison.Ordinal ) );
			var result = new List<UserCacheEntry>();

			foreach( var key in keys )
			{
				var json = await Cache.Get( key );
				var user = JsonConvert.DeserializeObject<UserCacheEntry>( json );

				result.Add( user );
			}

			return result;
		}

		public async Task<LinqToTwitter.Configuration> ReadTwitterConfig()
		{
			var json = await Cache.Get( ConfigurationKey );
			if( json == null )
			{
				return null;
			}

			return JsonConvert.DeserializeObject<LinqToTwitter.Configuration>( json );
		}

		public async Task SaveTwitterConfig( LinqToTwitter.Configuration cfg )
		{
			var json = JsonConvert.SerializeObject( cfg );
			await Cache.Set( ConfigurationKey, json, TimeSpan.FromDays( 1 ) );
		}

		private const string ConfigurationKey = "twitter.help.configuration";
		private readonly ICache Cache;
	}
}