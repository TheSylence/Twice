using LinqToTwitter;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Accounts
{
	internal class AuthorizeResult
	{
		public AuthorizeResult( TwitterAccountData data, IAuthorizer auth )
		{
			Data = data;
			Auth = auth;
		}

		public IAuthorizer Auth { get; }
		public TwitterAccountData Data { get; }
	}
}