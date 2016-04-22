using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class AddColumnDialogViewModel : WizardViewModel, IAddColumnDialogViewModel
	{
		public AddColumnDialogViewModel( ITwitterContextList contextList, IColumnDefinitionList columnList, ITimerFactory timerFactory )
		{
			ColumnList = columnList;

			Pages.Add( 0, new SourceAccountSelectorPage( contextList ) );
			Pages.Add( 1, new ColumnTypeSelctorPage() );
			Pages.Add( 2, new UserSelectorPage( contextList, timerFactory ) );
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
			var page = CurrentPage;
			var targetAccounts = GetProperty<ulong[]>( TargetAccountsKey ).ToList();
			var sourceAccounts = GetProperty<ulong[]>( SourceAccountsKey );

			int pageKey = 3;

			switch( type )
			{
			case ColumnType.Activity:
			case ColumnType.Favorites:
			case ColumnType.Timeline:
			case ColumnType.Mentions:
			case ColumnType.Messages:
				targetAccounts.AddRange( sourceAccounts );
				SetProperty( TargetAccountsKey, targetAccounts.ToArray() );
				break;

			case ColumnType.User:
				pageKey = 2;
				break;
			}

			SetProperty( ColumnTypeKey, type );
			page.SetNextPage( pageKey );
			GotoNextPageCommand.Execute( null );
		}

		private void ExecuteSelectAccountCommand( ulong accountId )
		{
			List<ulong> sourceAccounts = new List<ulong>
			{
				accountId
			};

			int pageKey = 1;
			SetProperty( TargetAccountsKey, new ulong[0] );
			SetProperty( SourceAccountsKey, sourceAccounts.ToArray() );
			CurrentPage.SetNextPage( pageKey );
			GotoNextPageCommand.Execute( null );
		}

		private void ExecuteSelectUserCommand( ulong userId )
		{
			var targetAccounts = GetProperty<ulong[]>( TargetAccountsKey ).ToList();
			targetAccounts.Add( userId );
			int pageKey = 3;
			CurrentPage.SetNextPage( pageKey );
			SetProperty( TargetAccountsKey, targetAccounts.ToArray() );
			GotoNextPageCommand.Execute( null );
		}

		public ICommand AddColumnTypeCommand => _AddColumnTypeCommand ?? ( _AddColumnTypeCommand = new RelayCommand<ColumnType>( ExecuteAddColumnTypeCommand ) );

		public ICommand SelectAccountCommand => _SelectAccountCommand ?? ( _SelectAccountCommand = new RelayCommand<ulong>( ExecuteSelectAccountCommand ) );

		public ICommand SelectUserCommand => _SelectUserCommand ?? ( _SelectUserCommand = new RelayCommand<ulong>( ExecuteSelectUserCommand ) );

		private const string ColumnTypeKey = "ColumnType";

		private const string SourceAccountsKey = "SourceAccounts";

		private const string TargetAccountsKey = "TargetAccounts";

		private readonly IColumnDefinitionList ColumnList;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand<ColumnType> _AddColumnTypeCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand<ulong> _SelectAccountCommand;

		[System.Diagnostics.DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand<ulong> _SelectUserCommand;
	}
}