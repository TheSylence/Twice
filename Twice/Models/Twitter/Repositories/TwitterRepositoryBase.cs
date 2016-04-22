using System.Diagnostics.CodeAnalysis;
using LinqToTwitter;

namespace Twice.Models.Twitter.Repositories
{
	[ExcludeFromCodeCoverage]
	internal abstract class TwitterRepositoryBase
	{
		protected TwitterRepositoryBase( TwitterContext context )
		{
			Context = context;
		}

		protected readonly TwitterContext Context;
	}
}