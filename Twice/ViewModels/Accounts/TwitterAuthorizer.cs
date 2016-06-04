using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Anotar.NLog;
using LinqToTwitter;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Accounts
{
	[ExcludeFromCodeCoverage]
	internal class TwitterAuthorizer : ITwitterAuthorizer
	{
		public async Task<AuthorizeResult> Authorize( Action<string> displayPinPageAction, Func<string> getPinAction,
			CancellationToken? token = null )
		{
			var auth = new PinAuthorizer
			{
				CredentialStore = new InMemoryCredentialStore
				{
					ConsumerKey = Constants.Auth.ConsumerKey,
					ConsumerSecret = Constants.Auth.ConsumerSecret
				},
				GoToTwitterAuthorization = displayPinPageAction,
				GetPin = getPinAction
			};

			try
			{
				await auth.AuthorizeAsync().ConfigureAwait( false );
			}
			catch( TwitterQueryException ex )
			{
				if( token?.IsCancellationRequested != true )
				{
					LogTo.ErrorException( "Failed to authorize user", ex );
				}

				return null;
			}

			var accountData = new TwitterAccountData
			{
				OAuthToken = auth.CredentialStore.OAuthToken,
				OAuthTokenSecret = auth.CredentialStore.OAuthTokenSecret,
				AccountName = auth.CredentialStore.ScreenName,
				UserId = auth.CredentialStore.UserID
			};

			return new AuthorizeResult( accountData, auth );
		}
	}
}