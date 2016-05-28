using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class AddColumnDialogViewModel : WizardViewModel, IAddColumnDialogViewModel
	{
		public AddColumnDialogViewModel( ITwitterContextList contextList, IColumnDefinitionList columnList,
			ITimerFactory timerFactory )
		{
			ColumnList = columnList;

			Pages.Add( 0, new SourceAccountSelectorPage( this, contextList ) );
			Pages.Add( 1, new ColumnTypeSelctorPage( this ) );
			Pages.Add( 2, new UserSelectorPage( this, timerFactory ) );
			Pages.Add( 3, new FinishPage( this ) );

			SetProperty( SourceAccountsKey, new ulong[0] );
			SetProperty( TargetAccountsKey, new ulong[0] );
			SetProperty( SourceAccountNamesKey, string.Empty );
			SetProperty( TargetAccountNamesKey, string.Empty );
			SetProperty( ContextsKey, new IContextEntry[0] );

			CurrentPage = Pages[0];
		}

		protected override void ExecuteFinishCommand()
		{
			var type = GetProperty<ColumnType>( ColumnTypeKey );
			var sourceAccounts = GetProperty<ulong[]>( SourceAccountsKey );
			var targetAccounts = GetProperty<ulong[]>( TargetAccountsKey );

			var newCol = ColumnDefinitionFactory.Construct( type, sourceAccounts, targetAccounts );

			ColumnList.AddColumns( new[] {newCol} );

			base.ExecuteFinishCommand();
		}

		internal const string ColumnTypeKey = "ColumnType";
		internal const string ContextsKey = "Context";
		internal const string SourceAccountNamesKey = "SourceAccountNames";
		internal const string SourceAccountsKey = "SourceAccounts";
		internal const string TargetAccountNamesKey = "TargetAccountNames";
		internal const string TargetAccountsKey = "TargetAccounts";
		private readonly IColumnDefinitionList ColumnList;
	}
}