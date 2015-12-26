using System;
using System.IO;
using LinqToTwitter;

namespace Twice.Models.Contexts
{
	internal class ContextEntry : IContextEntry
	{
		public ContextEntry()
		{
			var lines = File.ReadAllLines( "DebugTokens.txt" );

			AccountName = lines[0];
			UserId = ulong.Parse( lines[1] );

			Context = new TwitterContext( new SingleUserAuthorizer
			{
				CredentialStore = new InMemoryCredentialStore
				{
					ScreenName = AccountName,
					UserID = UserId,
					ConsumerKey = lines[2],
					ConsumerSecret = lines[3],
					OAuthToken = lines[4],
					OAuthTokenSecret = lines[5]
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
				Context?.Dispose();
			}
		}

		public string AccountName { get; }
		public TwitterContext Context { get; }
		public ulong UserId { get; }
	}
}