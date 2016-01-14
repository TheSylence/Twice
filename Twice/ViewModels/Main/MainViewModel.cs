using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.Models.Twitter;
using Twice.Services.ViewServices;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Main
{
	internal class MainViewModel : ViewModelBaseEx, IMainViewModel
	{
		public MainViewModel( ITwitterContextList list )
		{
			var context = list.Contexts.First();

			Columns = new ObservableCollection<IColumnViewModel>();

			Columns.Add( new UserColumn( context, context.UserId ) );
			Columns.Add( new TimelineColumn( context ) );
			Columns.Add( new UserColumn( context, 548023302 ) );
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