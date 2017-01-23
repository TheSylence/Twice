using Anotar.NLog;
using Fody;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			Search = new TwitterSearchRepository( context, cache );
			Messages = new TwitterMessageRepository( context, cache );
			Favorites = new TwitterFavoritesRepository( context, cache );
		}

		public Task<User> CreateBlockAsync( ulong userId, string screenName, bool skipStatus )
		{
			return Context.CreateBlockAsync( userId, screenName, skipStatus );
		}

		public void Dispose()
		{
			Context.Dispose();
		}

		public string GetAuthorizationString( string requestUrl, string method = "GET" )
		{
			var parameters = new Dictionary<string, string>
			{
				{ "oauth_token", Context.Authorizer.CredentialStore.OAuthToken },
				{ "oauth_consumer_key", Context.Authorizer.CredentialStore.ConsumerKey }
			};

			return Context.Authorizer.GetAuthorizationString( method, requestUrl, parameters );
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
				foreach( var category in response.RateLimits )
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

		public Task<LinqToTwitter.Media> UploadMediaAsync( byte[] mediaData, string mediaType,
			IEnumerable<ulong> additionalOwners )
		{
			return Context.UploadMediaAsync( mediaData, mediaType, additionalOwners, "tweet_image" );
		}

		public async Task<bool> VerifyCredentials()
		{
			var verifyResponse =
				await Context.Account.Where( a => a.Type == AccountType.VerifyCredentials ).SingleOrDefaultAsync();

			return verifyResponse?.User != null;
		}

		public IAuthorizer Authorizer => Context.Authorizer;
		public ITwitterFavoritesRepository Favorites { get; }
		public ITwitterFriendshipRepository Friendships { get; }
		public ITwitterMessageRepository Messages { get; }
		public ITwitterSearchRepository Search { get; }
		public ITwitterStatusRepository Statuses { get; }
		public ITwitterStreamingRepository Streaming { get; }
		public ITwitterUserRepository Users { get; }
		private readonly TwitterContext Context;
	}
}