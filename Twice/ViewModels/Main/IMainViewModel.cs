using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Main
{
	internal interface ILoadCallback
	{
		Task OnLoad( object data );
	}

	internal interface IMainViewModel : IViewModelBase, ILoadCallback
	{
		ICommand AccountsCommand { get; }
		ICollection<IColumnViewModel> Columns { get; }
		ICommand InfoCommand { get; }
		ICommand AddColumnCommand { get; }
		ICommand NewTweetCommand { get; }
		ICommand SettingsCommand { get; }
	}
}