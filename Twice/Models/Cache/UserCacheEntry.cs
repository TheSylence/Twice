using System.Data.Common;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Newtonsoft.Json;
using Twice.Models.Twitter;

namespace Twice.Models.Cache
{
	internal class UserCacheEntry
	{
		public UserCacheEntry( User user )
		{
			UserId = user.GetUserId();
			UserName = user.GetScreenName();
			Data = JsonConvert.SerializeObject( user );
		}

		public UserCacheEntry( UserMentionEntity mention )
		{
			UserId = mention.Id;
			UserName = mention.ScreenName;
		}

		private UserCacheEntry( ulong userId, string userName, string data )
		{
			UserId = userId;
			UserName = userName;
			Data = data;
		}

		[ConfigureAwait( false )]
		internal static async Task<UserCacheEntry> Read( DbDataReader reader )
		{
			var id = reader.GetInt64( 0 );
			var name = await reader.GetFieldValueAsync<string>( 1 );
			var data = await reader.GetFieldValueAsync<string>( 2 );

			return new UserCacheEntry( (ulong)id, name, data );
		}

		public string Data { get; }
		public ulong UserId { get; }
		public string UserName { get; }
	}
}