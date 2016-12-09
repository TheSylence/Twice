using System;
using Twice.Views.Dialogs;

namespace Twice.ViewModels.Dialogs.Data
{
	class SearchDialogData : DialogData
	{
		public SearchDialogData( string searchQuery )
			: base( typeof(SearchDialog), typeof(ISearchDialogViewModel))
		{
			SearchQuery = searchQuery;
		}

		public override bool Equals( DialogData obj )
		{
			var other = obj as SearchDialogData;
			return SearchQuery?.Equals( other?.SearchQuery, StringComparison.Ordinal ) == true;
		}

		public string SearchQuery { get; }
	}
}