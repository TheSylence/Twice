using Twice.Utilities;

namespace Twice.Models.Twitter
{
	class TwitterAccountData
	{
		public ulong UserId { get; set; }
		public string AccountName { get; set; }
		public string ImageUrl { get; set; }
		public string OAuthToken { get; set; }
		public string OAuthTokenSecret { get; set; }

		public void Encrypt()
		{
			OAuthToken = DpApi.Encrypt( DpApi.KeyType.UserKey, OAuthToken );
			OAuthTokenSecret = DpApi.Encrypt( DpApi.KeyType.UserKey, OAuthTokenSecret );
		}

		public void Decrypt()
		{
			OAuthToken = DpApi.Decrypt( OAuthToken );
			OAuthTokenSecret = DpApi.Decrypt( OAuthTokenSecret );
		}
	}
}