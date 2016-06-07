using LinqToTwitter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterMessageRepository
	{
		Task<List<DirectMessage>> IncomingMessages();

		Task<List<DirectMessage>> OutgoingMessages();
	}
}