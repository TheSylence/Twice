using System;
using Twice.Utilities.Os;

namespace Twice.Models.Twitter
{
	internal class TwitterAccountData
	{
		public void Decrypt()
		{
			OAuthToken = DpApi.Decrypt( OAuthToken );
			OAuthTokenSecret = DpApi.Decrypt( OAuthTokenSecret );
		}

		public void Encrypt()
		{
			OAuthToken = DpApi.Encrypt( DpApi.KeyType.UserKey, OAuthToken );
			OAuthTokenSecret = DpApi.Encrypt( DpApi.KeyType.UserKey, OAuthTokenSecret );
		}

		public TResult ExecuteDecryptedAction<TResult>( Func<TwitterAccountData, TResult> action )
		{
			Decrypt();

			try
			{
				return action( this );
			}
			finally
			{
				Encrypt();
			}
		}

		public void ExecuteDecryptedAction( Action<TwitterAccountData> action )
		{
			Decrypt();

			try
			{
				action( this );
			}
			finally
			{
				Encrypt();
			}
		}

		public string AccountName { get; set; }
		public string ImageUrl { get; set; }
		public bool IsDefault { get; set; }
		public string OAuthToken { get; set; }
		public string OAuthTokenSecret { get; set; }
		public bool RequiresConfirm { get; set; }
		public ulong UserId { get; set; }
	}
}