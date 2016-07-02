using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Twice.Models.Twitter.Entities;

namespace Twice.Models.Cache
{
	[ExcludeFromCodeCoverage]
	internal class NullCache : ICache
	{
		public Task AddHashtags( IList<string> hashTags )
		{
			return Task.CompletedTask;
		}

		public Task AddMessages( IList<MessageCacheEntry> messages )
		{
			return Task.CompletedTask;
		}

		public Task AddStatuses( IList<Status> statuses )
		{
			return Task.CompletedTask;
		}

		public Task AddUsers( IList<UserCacheEntry> users )
		{
			return Task.CompletedTask;
		}

		public void Dispose()
		{
		}

		public Task<ulong> FindFriend( ulong friendId )
		{
			return Task.FromResult( 0ul );
		}

		public Task<IEnumerable<string>> GetKnownHashtags()
		{
			return Task.FromResult( Enumerable.Empty<string>() );
		}

		public Task<IEnumerable<UserCacheEntry>> GetKnownUsers()
		{
			return Task.FromResult( Enumerable.Empty<UserCacheEntry>() );
		}

		public Task<List<MessageCacheEntry>> GetMessages()
		{
			return Task.FromResult( new List<MessageCacheEntry>() );
		}

		public Task<Status> GetStatus( ulong id )
		{
			return Task.FromResult<Status>( null );
		}

		public Task<List<Status>> GetStatusesForColumn( Guid columnId, int limit )
		{
			return Task.FromResult( new List<Status>() );
		}

		public Task<List<Status>> GetStatusesForUser( ulong userId )
		{
			return Task.FromResult( new List<Status>() );
		}

		public Task<UserEx> GetUser( ulong userId )
		{
			return Task.FromResult<UserEx>( null );
		}

		public Task<IEnumerable<ulong>> GetUserFriends( ulong userId )
		{
			return Task.FromResult( Enumerable.Empty<ulong>() );
		}

		public Task MapStatusesToColumn( IList<Status> statuses, Guid columnId )
		{
			return Task.CompletedTask;
		}

		public Task<LinqToTwitter.Configuration> ReadTwitterConfig()
		{
			return Task.FromResult<LinqToTwitter.Configuration>( null );
		}

		public Task RemoveStatus( ulong id )
		{
			return Task.CompletedTask;
		}

		public Task SaveTwitterConfig( LinqToTwitter.Configuration cfg )
		{
			return Task.CompletedTask;
		}

		public Task SetUserFriends( ulong userId, IEnumerable<ulong> friendIds )
		{
			return Task.CompletedTask;
		}
	}
}