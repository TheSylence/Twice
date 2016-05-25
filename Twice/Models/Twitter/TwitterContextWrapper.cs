using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anotar.NLog;
using Fody;
using LinqToTwitter;
using Twice.Models.Cache;
using Twice.Models.Twitter.Repositories;

namespace Twice.Models.Twitter
{
	[ExcludeFromCodeCoverage]
	[ConfigureAwait( false )]
	internal class TwitterContextWrapper : ITwitterContext
	{
		public TwitterContextWrapper( TwitterContext context, ICache cache )
		{
			Context = context;

			Users = new TwitterUserRepository( context, cache );
			Statuses = new TwitterStatusRepository( context, cache );
			Friendships = new TwitterFriendshipRepository( context, cache );
			Streaming = new TwitterStreamingRepository( context, cache );
		}

		public Task<User> CreateBlockAsync( ulong userId, string screenName, bool skipStatus )
		{
			return Context.CreateBlockAsync( userId, screenName, skipStatus );
		}

		public Task<Status> CreateFavoriteAsync( ulong statusId )
		{
			return Context.CreateFavoriteAsync( statusId );
		}

		public Task<Status> DeleteTweetAsync( ulong statusId )
		{
			return Context.DeleteTweetAsync( statusId );
		}

		public Task<Status> DestroyFavoriteAsync( ulong statusId )
		{
			return Context.DestroyFavoriteAsync( statusId );
		}

		public void Dispose()
		{
			Context.Dispose();
		}

		public async Task<LinqToTwitter.Configuration> GetConfig()
		{
			var help = await Context.Help.Where( h => h.Type == HelpType.Configuration ).SingleOrDefaultAsync();

			return help.Configuration;
		}

		public async Task LogCurrentRateLimits()
		{
			var response = await Context.Help.Where( h => h.Type == HelpType.RateLimits ).SingleOrDefaultAsync();

			if( response?.RateLimits != null )
			{
				var sb = new StringBuilder();
				sb.AppendLine();
				
				sb.AppendLine( "--- RATE LIMITS START ---" );
				foreach( var category in response?.RateLimits )
				{
					var limits = category.Value.Where( l => l.Limit != l.Remaining ).ToArray();
					if( !limits.Any() )
					{
						continue;
					}

					sb.AppendLine( $"Category: {category.Key}" );

					foreach( var limit in limits )
					{
						var nextReset = limit.Reset.AsUnixTimestamp() - DateTime.Now;

						sb.AppendLine( $"\t{limit.Resource} => ({limit.Remaining}/{limit.Limit}) => Reset in {nextReset:mm\\:ss} minutes" );
					}

					sb.AppendLine();
				}
				sb.AppendLine( "--- RATE LIMITS END ---" );

				LogTo.Debug( sb.ToString() );
			}
		}

		public async Task ReportAsSpam( ulong userId )
		{
			await Context.ReportSpamAsync( userId );
		}

		public Task<Status> RetweetAsync( ulong statusId )
		{
			return Context.RetweetAsync( statusId );
		}

		public Task<Status> TweetAsync( string text, IEnumerable<ulong> medias )
		{
			return Context.TweetAsync( text, medias );
		}

		public Task<LinqToTwitter.Media> UploadMediaAsync( byte[] mediaData, string mediaType, IEnumerable<ulong> additionalOwners )
		{
			return Context.UploadMediaAsync( mediaData, mediaType, additionalOwners );
		}

		public IAuthorizer Authorizer => Context.Authorizer;
		public ITwitterFriendshipRepository Friendships { get; }
		public ITwitterStatusRepository Statuses { get; }
		public ITwitterStreamingRepository Streaming { get; }
		public ITwitterUserRepository Users { get; }
		private readonly TwitterContext Context;
	}
}