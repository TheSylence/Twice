using System.Linq;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterStreamingRepository
	{
		IQueryable<LinqToTwitter.Streaming> GetUserStream();
	}
}