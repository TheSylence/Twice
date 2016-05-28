using LinqToTwitter;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Models.Cache;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	internal class TwitterStreamingRepository : TwitterRepositoryBase, ITwitterStreamingRepository
	{
		public TwitterStreamingRepository( TwitterContext context, ICache cache )
			: base( context, cache )
		{
		}

		public IQueryable<LinqToTwitter.Streaming> GetUserStream()
		{
			return Context.Streaming.Where( s => s.Type == StreamingType.User );
		}
	}
}