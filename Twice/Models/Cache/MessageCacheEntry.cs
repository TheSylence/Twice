using System.Data.Common;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Newtonsoft.Json;
using Twice.Models.Twitter;

namespace Twice.Models.Cache
{
	internal class MessageCacheEntry
	{
		public MessageCacheEntry( DirectMessage message )
		{
			Data = JsonConvert.SerializeObject( message );
			Sender = message.SenderID;
			Recipient = message.RecipientID;
			Id = message.GetMessageId();
		}

		private MessageCacheEntry( ulong id, ulong sender, ulong recipient, string data )
		{
			Id = id;
			Sender = sender;
			Recipient = recipient;
			Data = data;
		}

		[ConfigureAwait( false )]
		internal static async Task<MessageCacheEntry> Read( DbDataReader reader )
		{
			var id = reader.GetInt64( 0 );
			var sender = reader.GetInt64( 1 );
			var recipient = reader.GetInt64( 2 );

			string data = string.Empty;
			if( !await reader.IsDBNullAsync( 3 ) )
			{
				data = await reader.GetFieldValueAsync<string>( 3 );
			}

			return new MessageCacheEntry( (ulong)id, (ulong)sender, (ulong)recipient, data );
		}

		public string Data { get; set; }
		public ulong Id { get; set; }
		public ulong Recipient { get; set; }
		public ulong Sender { get; set; }
	}
}