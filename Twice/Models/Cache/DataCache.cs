using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleCacheSharp;

namespace Twice.Models.Cache
{
	internal class DataCache : IDataCache
	{
		public DataCache( ICache cache )
		{
			Cache = cache;
		}

		[ExcludeFromCodeCoverage]
		public DataCache( string fileName )
		{
			FileName = fileName;
		}

		[ExcludeFromCodeCoverage]
		private async Task ConstructCache()
		{
			if( Cache != null )
			{
				return;
			}

			Cache = await CacheFactory.Configure().UsingFile( FileName ).BuildCache();
		}

		public async Task AddHashtag( string hashtag )
		{
			await ConstructCache();

			var data = new HashtagCacheEntry( hashtag );
			var key = data.GetKey();
			var value = JsonConvert.SerializeObject( data );

			await Cache.Set( key, value, Constants.Cache.HashtagExpiration );
		}

		public async Task AddUser( ulong id, string name )
		{
			await ConstructCache();

			UserCacheEntry data = new UserCacheEntry( id, name );
			var key = data.GetKey();
			var value = JsonConvert.SerializeObject( data );

			await Cache.Set( key, value, Constants.Cache.UserInfoExpiration );
		}

		public async Task<IEnumerable<string>> GetKnownHashtags()
		{
			await ConstructCache();

			var keys = ( await Cache.GetKeys() ).Where( k => k.StartsWith( $"{HashtagCacheEntry.Key}:", StringComparison.Ordinal ) );
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
			var users = await GetKnownUsers();
			return users.Select( u => u.Id );
		}

		public async Task<IEnumerable<UserCacheEntry>> GetKnownUsers()
		{
			await ConstructCache();

			var keys = ( await Cache.GetKeys() ).Where( k => k.StartsWith( $"{UserCacheEntry.Key}:", StringComparison.Ordinal ) );
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
			await ConstructCache();

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

		internal const string ConfigurationKey = "twitter.help.configuration";
		private readonly string FileName;
		private ICache Cache;
	}
}