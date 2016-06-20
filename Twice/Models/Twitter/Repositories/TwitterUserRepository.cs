using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using LitJson;
using Twice.Models.Cache;
using Twice.Models.Twitter.Entities;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	[ConfigureAwait( false )]
	internal class TwitterUserRepository : TwitterRepositoryBase, ITwitterUserRepository
	{
		public TwitterUserRepository( TwitterContext context, ICache cache )
			: base( context, cache )
		{
		}

		private async Task<UserEx> LoadUserFromQuery( string queryString )
		{
			Raw rawResult;
			try
			{
				rawResult = await Context.RawQuery.Where( q => q.QueryString == queryString ).SingleOrDefaultAsync();
				if( rawResult == null )
				{
					return null;
				}
			}
			catch( TwitterQueryException )
			{
				return null;
			}

			var json = JsonMapper.ToObject( rawResult.Response );
			var user = new UserEx( json );

			await Cache.AddUsers( new[] {new UserCacheEntry( user )} );

			return user;
		}

		public async Task FollowUser( ulong userId )
		{
			await Context.CreateFriendshipAsync( userId, true );
		}

		public Task<List<UserEx>> LookupUsers( IEnumerable<ulong> userIds )
		{
			var userList = string.Join( ",", userIds );

			return LookupUsers( userList );
		}

		public async Task<List<UserEx>> LookupUsers( string userList )
		{
			if( string.IsNullOrEmpty( userList ) )
			{
				return new List<UserEx>();
			}

			var idStr = Uri.EscapeDataString( userList );
			string queryString = $"users/lookup.json?user_id={idStr}&include_entities=true";
			Raw rawResult;
			try
			{
				rawResult = await Context.RawQuery.Where( q => q.QueryString == queryString ).SingleOrDefaultAsync();
				if( rawResult == null )
				{
					return new List<UserEx>();
				}
			}
			catch( TwitterQueryException )
			{
				return new List<UserEx>();
			}

			var json = JsonMapper.ToObject( rawResult.Response );
			List<UserEx> users = json.Select( j => new UserEx( j.Value ) ).ToList();

			await Cache.AddUsers( users.Select( u => new UserCacheEntry( u ) ).ToList() );
			return users;
		}

		public async Task<UserEx> ShowUser( string screenName, bool includeEntities )
		{
			// TODO: Caching?

			var idStr = Uri.EscapeDataString( screenName );
			string queryString = $"users/show.json?screen_name={idStr}";
			if( includeEntities )
			{
				queryString += "&include_entities=true";
			}

			return await LoadUserFromQuery( queryString );
		}

		public async Task<UserEx> ShowUser( ulong userId, bool includeEntities )
		{
			var cached = await Cache.GetUser( userId );
			if( cached != null )
			{
				return cached;
			}

			var idStr = Uri.EscapeDataString( userId.ToString() );
			string queryString = $"users/show.json?user_id={idStr}";
			if( includeEntities )
			{
				queryString += "&include_entities=true";
			}

			return await LoadUserFromQuery( queryString );
		}

		public async Task UnfollowUser( ulong userId )
		{
			await Context.DestroyFriendshipAsync( userId );
		}
	}
}