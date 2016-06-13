using System.Collections.Generic;
using System.Windows.Input;

namespace Twice.ViewModels.Dialogs
{
	internal interface ISearchDialogViewModel : IDialogViewModel
	{
		SearchMode Mode { get; set; }
		ICommand SearchCommand { get; }
		string SearchQuery { get; set; }
		ICollection<object> SearchResults { get; }
		bool IsSearching { get; }
	}
}