using System.Collections.Generic;
using System.Linq;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class SourceAccountSelectorPage : WizardPageViewModel
	{
		public SourceAccountSelectorPage( IWizardViewModel wizard, ITwitterContextList contextList )
			: base( wizard )
		{
			Accounts = contextList.Contexts.Select( c => new AccountEntry( c ) ).ToList();
		}

		protected override void ExecuteGotoNextPageCommand( object args )
		{
			ulong accountId = (ulong)args;
			List<ulong> sourceAccounts = new List<ulong>
			{
				accountId
			};

			var accounts = sourceAccounts.Select( id => Accounts.Single( acc => acc.Context.UserId == id ) ).ToArray();

			int pageKey = 1;
			Wizard.SetProperty( AddColumnDialogViewModel.TargetAccountsKey, new ulong[0] );
			Wizard.SetProperty( AddColumnDialogViewModel.SourceAccountsKey, sourceAccounts.ToArray() );
			Wizard.SetProperty( AddColumnDialogViewModel.SourceAccountNamesKey,
				accounts.Select( acc => acc.ScreenName ).ToArray() );
			Wizard.SetProperty( AddColumnDialogViewModel.ContextsKey, accounts.Select( acc => acc.Context ).ToArray() );
			Wizard.GotoPage( pageKey );
		}

		public ICollection<AccountEntry> Accounts { get; }
	}
}