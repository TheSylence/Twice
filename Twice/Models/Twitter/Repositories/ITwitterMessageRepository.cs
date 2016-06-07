using LinqToTwitter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterMessageRepository
	{
		Task<List<DirectMessage>> IncomingMessages( int count = 50, ulong? maxId = null );

		Task<List<DirectMessage>> OutgoingMessages( int count = 50, ulong? maxId = null );
	}
}