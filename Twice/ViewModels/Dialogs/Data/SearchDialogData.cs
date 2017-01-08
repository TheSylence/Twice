using System;
using Twice.Views.Dialogs;

namespace Twice.ViewModels.Dialogs.Data
{
	internal class SearchDialogData : DialogData
	{
		public SearchDialogData( string searchQuery )
			: base( typeof( SearchDialog ), typeof( ISearchDialogViewModel ) )
		{
			SearchQuery = searchQuery;
		}

		public override bool Equals( DialogData obj )
		{
			var other = obj as SearchDialogData;
			return SearchQuery?.Equals( other?.SearchQuery, StringComparison.Ordinal ) == true;
		}

		public override object GetResult( object viewModel )
		{
			return null;
		}

		public override void Setup( object viewModel )
		{
			var vm = CastViewModel<ISearchDialogViewModel>( viewModel );

			if( !string.IsNullOrWhiteSpace( SearchQuery ) )
			{
				vm.SearchQuery = SearchQuery;
				vm.SearchCommand.Execute( null );
			}
		}

		public string SearchQuery { get; }
	}
}