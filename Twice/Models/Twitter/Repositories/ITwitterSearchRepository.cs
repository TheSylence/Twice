using Fody;
using LinqToTwitter;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Twice.Models.Cache;

namespace Twice.Models.Twitter.Repositories
{
	internal interface ITwitterSearchRepository
	{
		Task<List<Status>> SearchReplies( Status status );
	}

	[ExcludeFromCodeCoverage]
	[ConfigureAwait( false )]
	internal class TwitterSearchRepository : TwitterRepositoryBase, ITwitterSearchRepository
	{
		public TwitterSearchRepository( TwitterContext context, ICache cache )
			: base( context, cache )
		{
		}

		public async Task<List<Status>> SearchReplies( Status status )
		{
			var searchQuery = $"to:@{status.User.GetScreenName()}";

			var searchResult =
				await
					Queryable.Where( s => s.ResultType == ResultType.Mixed && s.Query == searchQuery && s.Type == SearchType.Search )
						.SingleOrDefaultAsync();

			List<Status> replies = new List<Status>();

			if( searchResult?.Statuses != null )
			{
				foreach( var s in searchResult.Statuses )
				{
					if( s.InReplyToStatusID == status.GetStatusId() )
					{
						replies.Add( s );
					}
				}
			}

			return replies;
		}

		public TwitterQueryable<Search> Queryable => Context.Search;
	}
}