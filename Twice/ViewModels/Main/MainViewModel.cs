using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.Messages;
using Twice.Models.Twitter;
using Twice.Services.ViewServices;
using Twice.ViewModels.Columns;
using Twice.ViewModels.Columns.Definitions;
using Twice.Views;

namespace Twice.ViewModels.Main
{
	internal class MainViewModel : ViewModelBaseEx, IMainViewModel
	{
		public MainViewModel( ITwitterContextList list )
		{
			var context = list.Contexts.First();

			var columnList = new ColumnDefintionList( Constants.IO.ColumnDefintionFileName );
			var columns = columnList.Load();
			if( !columns.Any() )
			{
				columns = columnList.DefaultColumns( context.UserId );
			}

			var factory = new ColumnFactory( list );

			var constructed = columns.Select( c => factory.Construct( c ) );
#if DEBUG
			constructed = constructed.Where( c => c != null );
#endif

			Columns = new ObservableCollection<IColumnViewModel>( constructed );
		}

		public async Task OnLoad()
		{
			foreach( var col in Columns )
			{
				await col.Load();
			}
		}

		private void ExecuteNewTweetCommand()
		{
			MessengerInstance.Send( new FlyoutMessage( FlyoutNames.TweetComposer, FlyoutAction.Open ) );
		}

		private async void ExecuteSettingsCommand()
		{
			await ServiceRepository.Show<ISettingsService, object>();
		}

		public ICollection<IColumnViewModel> Columns { get; }

		public ICommand NewTweetCommand => _NewTweetCommand ?? ( _NewTweetCommand = new RelayCommand( ExecuteNewTweetCommand ) );

		public ICommand SettingsCommand => _SettingsCommand ?? ( _SettingsCommand = new RelayCommand( ExecuteSettingsCommand ) );

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _NewTweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _SettingsCommand;
	}
}