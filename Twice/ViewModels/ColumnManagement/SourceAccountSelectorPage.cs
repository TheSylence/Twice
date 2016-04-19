using System.Collections.Generic;
using System.Linq;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class SourceAccountSelectorPage : WizardPageViewModel
	{
		public SourceAccountSelectorPage( ITwitterContextList contextList )
		{
			Accounts = contextList.Contexts.Select( c => new AccountEntry( c ) ).ToList();
		}

		public ICollection<AccountEntry> Accounts { get; }

		public override int NextPageKey { get; protected set; } = -1;
	}
}