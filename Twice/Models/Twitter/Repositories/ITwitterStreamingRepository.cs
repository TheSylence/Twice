using System.Linq;

namespace Twice.Models.Twitter.Repositories
{
	interface ITwitterStreamingRepository
	{
		IQueryable<LinqToTwitter.Streaming> GetUserStream();
	}
}