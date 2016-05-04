using System.Collections.Generic;
using System.Linq;
using Twice.Models.Columns;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class ColumnTypeSelctorPage : WizardPageViewModel
	{
		public ColumnTypeSelctorPage( IWizardViewModel wizard )
			: base( wizard )
		{
			ColumnTypes = new List<ColumnTypeItem>( ColumnTypeListFactory.GetItems() );
		}

		protected override void ExecuteGotoNextPageCommand( object args )
		{
			var type = (ColumnType)args;

			var targetAccounts = Wizard.GetProperty<ulong[]>( AddColumnDialogViewModel.TargetAccountsKey ).ToList();
			var sourceAccounts = Wizard.GetProperty<ulong[]>( AddColumnDialogViewModel.SourceAccountsKey );

			int pageKey = 3;

			switch( type )
			{
			case ColumnType.Activity:
			case ColumnType.Favorites:
			case ColumnType.Timeline:
			case ColumnType.Mentions:
			case ColumnType.Messages:
				targetAccounts.AddRange( sourceAccounts );
				Wizard.SetProperty( AddColumnDialogViewModel.TargetAccountsKey, targetAccounts.ToArray() );
				break;

			case ColumnType.User:
				pageKey = 2;
				break;
			}

			Wizard.SetProperty( AddColumnDialogViewModel.ColumnTypeKey, type );
			Wizard.GotoPage( pageKey );
		}

		public ICollection<ColumnTypeItem> ColumnTypes { get; }
	}
}