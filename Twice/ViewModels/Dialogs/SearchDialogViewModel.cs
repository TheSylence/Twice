using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Fody;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.Resources;
using Twice.ViewModels.Twitter;

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

			await Dispatcher.RunAsync( Center );
			IsSearching = false;
		}

		public bool IsSearching { get; set; }

		public SearchMode Mode { get; set; }

		public ICommand SearchCommand => _SearchCommand ?? ( _SearchCommand = new RelayCommand(
			                                 ExecuteSearchCommand,
			                                 CanExecuteSearchCommand ) );

		public string SearchQuery { get; set; }

		public ICollection<object> SearchResults { get; }

		private RelayCommand _SearchCommand;
	}
}