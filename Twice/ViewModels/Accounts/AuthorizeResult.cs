using LinqToTwitter;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Accounts
{
	class AuthorizeResult
	{
		public AuthorizeResult( TwitterAccountData data, IAuthorizer auth )
		{
			Data = data;
			Auth = auth;
		}

		public TwitterAccountData Data { get; }
		public IAuthorizer Auth { get; }
	}
}