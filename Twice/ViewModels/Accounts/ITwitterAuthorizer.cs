using System;
using System.Threading;
using System.Threading.Tasks;

namespace Twice.ViewModels.Accounts
{
	interface ITwitterAuthorizer
	{
		Task<AuthorizeResult> Authorize( Action<string> displayPinPageAction, Func<string> getPinAction, CancellationToken? token );
	}
}