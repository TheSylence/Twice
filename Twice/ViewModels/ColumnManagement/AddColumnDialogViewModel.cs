using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{

	internal class AddColumnDialogViewModel : WizardViewModel, IAddColumnDialogViewModel
	{
		public AddColumnDialogViewModel( ITwitterContextList contextList )
		{
			Pages.Add( 0, new ColumnTypeSelctorPage() );
			Pages.Add( 1, new SourceAccountSelectorPage( contextList ) );
			Pages.Add( 2, new UserSelectorPage() );
			  
			CurrentPage = Pages[0];
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

			page.SetNextPage( pageKey );
			GotoNextPageCommand.Execute( null );
		}

		public ICommand AddColumnTypeCommand => _AddColumnTypeCommand ?? ( _AddColumnTypeCommand = new RelayCommand<ColumnType>( ExecuteAddColumnTypeCommand ) );

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand<ColumnType> _AddColumnTypeCommand;
	}
}