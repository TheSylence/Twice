using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterMessageRepository
	{
		Task<DirectMessage> SendMessage( string recipient, string message );

		Task<List<DirectMessage>> IncomingMessages( int count = 50, ulong? maxId = null );

		Task<List<DirectMessage>> OutgoingMessages( int count = 50, ulong? maxId = null );
	}
}