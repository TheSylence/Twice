using LinqToTwitter;
using System;
using Twice.Models.Cache;
using Twice.ViewModels;

namespace Twice.Models.Twitter
{
	internal class ContextEntry : IContextEntry
	{
		public ContextEntry( INotifier notifier, TwitterAccountData data, ICache cache )
		{
			Data = data;
			Notifier = notifier;

			AccountName = data.AccountName;
			UserId = data.UserId;
			ProfileImageUrl = new Uri( data.ImageUrl );
			IsDefault = data.IsDefault;
			RequiresConfirmation = data.RequiresConfirm;

			Twitter = new TwitterContextWrapper( new TwitterContext( new SingleUserAuthorizer
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
			} ), cache );
		}

		internal ContextEntry( INotifier notifier, TwitterAccountData data, ICache cache, ITwitterContext context )
		{
			Data = data;
			Notifier = notifier;
			Twitter = context;
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		public override bool Equals( object obj )
		{
			var other = obj as ContextEntry;
			return other?.UserId == UserId;
		}

		public override int GetHashCode()
		{
			return UserId.GetHashCode();
		}

		private void Dispose( bool disposing )
		{
			if( disposing )
			{
				Twitter?.Dispose();
			}
		}

		public string AccountName { get; }
		public TwitterAccountData Data { get; }
		public bool IsDefault { get; set; }
		public INotifier Notifier { get; }
		public Uri ProfileImageUrl { get; }
		public bool RequiresConfirmation { get; }
		public ITwitterContext Twitter { get; }
		public ulong UserId { get; }
	}
}