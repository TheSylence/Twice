using System.Diagnostics.CodeAnalysis;
using LinqToTwitter;
using Twice.Models.Cache;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	internal abstract class TwitterRepositoryBase
	{
		protected TwitterRepositoryBase( TwitterContext context, ICache cache )
		{
			Context = context;
			Cache = cache;
		}

		protected readonly ICache Cache;
		protected readonly TwitterContext Context;
	}
}