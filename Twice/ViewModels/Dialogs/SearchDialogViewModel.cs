using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Fody;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.ViewModels.Twitter;
using Twice.Resources;

namespace Twice.ViewModels.Dialogs
{
	[ConfigureAwait( false )]
	internal class SearchDialogViewModel : DialogViewModel, ISearchDialogViewModel
	{
		public SearchDialogViewModel()
		{
			Title = Strings.Search;

			SearchResults = new ObservableCollection<object>();
		}

		private bool CanExecuteSearchCommand()
		{
			return !string.IsNullOrWhiteSpace( SearchQuery );
		}

		private async void ExecuteSearchCommand()
		{
			IsSearching = true;
			var context = ContextList.Contexts.First();
			SearchResults.Clear();

			switch( Mode )
			{
				case SearchMode.Users:
				{
					var result = await context.Twitter.Search.SearchUsers( SearchQuery );
					var users = result.Select( u => new UserViewModel( u ) );

					await Dispatcher.RunAsync( () => SearchResults.AddRange( users ) );
				}
				break;

				case SearchMode.Statuses:
				{
					var result = await context.Twitter.Search.SearchStatuses( SearchQuery );
					var statuses = result.Select( s => new StatusViewModel( s, context, Configuration, ViewServiceRepository ) ).ToArray();
					await Task.WhenAll( statuses.Select( s => s.LoadDataAsync() ) );

					await Dispatcher.RunAsync( () => SearchResults.AddRange( statuses ) );
				}
				break;
			}

			IsSearching = false;
		}

		public bool IsSearching
		{
			[DebuggerStepThrough] get { return _IsSearching; }
			set
			{
				if( _IsSearching == value )
				{
					return;
				}

				_IsSearching = value;
				RaisePropertyChanged();
			}
		}

		public SearchMode Mode
		{
			[DebuggerStepThrough] get { return _Mode; }
			set
			{
				if( _Mode == value )
				{
					return;
				}

				_Mode = value;
				RaisePropertyChanged();
			}
		}

		public ICommand SearchCommand => _SearchCommand ?? ( _SearchCommand = new RelayCommand(
			ExecuteSearchCommand,
			CanExecuteSearchCommand ) );

		public string SearchQuery
		{
			[DebuggerStepThrough] get { return _SearchQuery; }
			set
			{
				if( _SearchQuery == value )
				{
					return;
				}

				_SearchQuery = value;
				RaisePropertyChanged();
			}
		}

		public ICollection<object> SearchResults { get; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsSearching;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private SearchMode _Mode;

		private RelayCommand _SearchCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _SearchQuery;
	}
}