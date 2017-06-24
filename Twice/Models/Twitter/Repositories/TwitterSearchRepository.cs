using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using Fody;
using LinqToTwitter;
using Twice.Models.Cache;

namespace Twice.Models.Twitter.Repositories
{
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

			Search searchResult;
			List<Status> replies = new List<Status>();

			try
			{
				searchResult =
					await
						Queryable.Where( s => s.ResultType == ResultType.Mixed && s.Query == searchQuery && s.Type == SearchType.Search )
							.SingleOrDefaultAsync();
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to search replies for status", ex );
				return replies;
			}

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

		public async Task<List<Status>> SearchStatuses( string query )
		{
			var searchResult =
				await
					Queryable.Where( s => s.Type == SearchType.Search && s.Query == query && s.ResultType == ResultType.Recent )
						.SingleOrDefaultAsync();

			List<Status> result = new List<Status>();
			if( searchResult?.Statuses != null )
			{
				foreach( var s in searchResult.Statuses )
				{
					result.Add( s );
				}
			}

			return result;
		}

		public Task<List<User>> SearchUsers( string query )
		{
			return Context.User.Where( s => s.Type == UserType.Search && s.Query == query ).ToListAsync();
		}

		private TwitterQueryable<Search> Queryable => Context.Search;
	}
}