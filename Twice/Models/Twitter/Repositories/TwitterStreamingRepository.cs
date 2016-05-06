using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LinqToTwitter;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	internal class TwitterStreamingRepository : TwitterRepositoryBase, ITwitterStreamingRepository
	{
		public TwitterStreamingRepository( TwitterContext context )
			: base( context )
		{
		}

		public IQueryable<LinqToTwitter.Streaming> GetUserStream()
		{
			return Context.Streaming.Where( s => s.Type == StreamingType.User );
		}
	}
}