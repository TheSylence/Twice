using System;
using LinqToTwitter;
using Twice.ViewModels;

namespace Twice.Models.Twitter
{
	internal class ContextEntry : IContextEntry
	{
		public ContextEntry( INotifier notifier, TwitterAccountData data )
		{
			Notifier = notifier;

			AccountName = data.AccountName;
			UserId = data.UserId;
			ProfileImageUrl = new Uri( data.ImageUrl );

			Twitter = new TwitterContext( new SingleUserAuthorizer
			{
				CredentialStore = new InMemoryCredentialStore
				{
					ScreenName = data.AccountName,
					UserID = data.UserId,
					ConsumerKey = Constants.Auth.ConsumerKey,
					ConsumerSecret = Constants.Auth.ConsumerSecret,
					OAuthToken = data.OAuthToken,
					OAuthTokenSecret = data.OAuthTokenSecret
				}
			} );
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		private void Dispose( bool disposing )
		{
			if( disposing )
			{
				Twitter?.Dispose();
			}
		}

		public string AccountName { get; }
		public INotifier Notifier { get; }
		public Uri ProfileImageUrl { get; }
		public TwitterContext Twitter { get; }
		public ulong UserId { get; }
	}
}