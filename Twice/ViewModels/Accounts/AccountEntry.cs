using System;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Accounts
{
	internal class AccountEntry
	{
		public AccountEntry( IContextEntry context )
		{
			AccountName = context.AccountName;
			ProfileImage = context.ProfileImageUrl;
			IsDefaultAccount = true;
		}

		public string AccountName { get; }
		public bool IsDefaultAccount { get; set; }
		public Uri ProfileImage { get; }
	}
}