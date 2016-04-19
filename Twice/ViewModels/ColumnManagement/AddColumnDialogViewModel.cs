using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class AddColumnDialogViewModel : WizardViewModel, IAddColumnDialogViewModel
	{
		public AddColumnDialogViewModel( ITwitterContextList contextList, IColumnDefinitionList columnList )
		{
			ColumnList = columnList;

			Pages.Add( 0, new ColumnTypeSelctorPage() );
			Pages.Add( 1, new SourceAccountSelectorPage( contextList ) );
			Pages.Add( 2, new UserSelectorPage() );
			Pages.Add( 3, new FinishPage() );

			SetProperty( SourceAccountsKey, new ulong[0] );
			SetProperty( TargetAccountsKey, new ulong[0] );

			CurrentPage = Pages[0];
		}

		protected override void ExecuteFinishCommand()
		{
			var type = GetProperty<ColumnType>( ColumnTypeKey );
			var sourceAccounts = GetProperty<ulong[]>( SourceAccountsKey );
			var targetAccounts = GetProperty<ulong[]>( TargetAccountsKey );

			var newCol = ColumnDefinitionFactory.Construct( type, sourceAccounts, targetAccounts );

			ColumnList.AddColumns( new[] { newCol } );

			base.ExecuteFinishCommand();
		}

		private void ExecuteAddColumnTypeCommand( ColumnType type )
		{
			var page = CurrentPage as ColumnTypeSelctorPage;

			int pageKey;

			switch( type )
			{
			case ColumnType.User:
				pageKey = 2;
				break;

			default:
				pageKey = 1;
				break;
			}

			SetProperty( ColumnTypeKey, type );
			page.SetNextPage( pageKey );
			GotoNextPageCommand.Execute( null );
		}

		private void ExecuteSelectAccountCommand( ulong accountId )
		{
			var columnType = GetProperty<ColumnType>( ColumnTypeKey );
			List<ulong> targetAccounts = new List<ulong>();
			List<ulong> sourceAccounts = new List<ulong>();

			int pageKey;

			switch( columnType )
			{
			case ColumnType.Activity:
			case ColumnType.Favorites:
			case ColumnType.Timeline:
			case ColumnType.Mentions:
			case ColumnType.Messages:
				targetAccounts.Add( accountId );
				sourceAccounts.Add( accountId );
				pageKey = 3;
				break;

			case ColumnType.User:
				sourceAccounts.Add( accountId );
				pageKey = 3;
				break;

			default:
				pageKey = 3;
				break;
			}

			SetProperty( TargetAccountsKey, targetAccounts.ToArray() );
			SetProperty( SourceAccountsKey, sourceAccounts.ToArray() );
			CurrentPage.SetNextPage( pageKey );
			GotoNextPageCommand.Execute( null );
		}

		public ICommand AddColumnTypeCommand => _AddColumnTypeCommand ?? ( _AddColumnTypeCommand = new RelayCommand<ColumnType>( ExecuteAddColumnTypeCommand ) );
		public ICommand SelectAccountCommand => _SelectAccountCommand ?? ( _SelectAccountCommand = new RelayCommand<ulong>( ExecuteSelectAccountCommand ) );
		private const string ColumnTypeKey = "ColumnType";
		private const string SourceAccountsKey = "SourceAccounts";

		private const string TargetAccountsKey = "TargetAccounts";

		private readonly IColumnDefinitionList ColumnList;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand<ColumnType> _AddColumnTypeCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand<ulong> _SelectAccountCommand;
	}
}