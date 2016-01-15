using System;
using System.IO;
using LinqToTwitter;

namespace Twice.Models.Twitter
{
	internal class ContextEntry : IContextEntry
	{
		public ContextEntry()
		{
			var lines = File.ReadAllLines( "DebugTokens.txt" );

			AccountName = lines[0];
			UserId = ulong.Parse( lines[1] );
			ProfileImageUrl = new Uri( lines[2] );

			Twitter = new TwitterContext( new SingleUserAuthorizer
			{
				CredentialStore = new InMemoryCredentialStore
				{
					ScreenName = AccountName,
					UserID = UserId,
					ConsumerKey = lines[3],
					ConsumerSecret = lines[4],
					OAuthToken = lines[5],
					OAuthTokenSecret = lines[6]
				}
			} );
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		void Dispose( bool disposing )
		{
			if( disposing )
			{
				Twitter?.Dispose();
			}
		}

		public Uri ProfileImageUrl { get; }
		public string AccountName { get; }
		public TwitterContext Twitter { get; }
		public ulong UserId { get; }
	}
}