using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Main
{
	interface ILoadCallback
	{
		Task OnLoad( object data );
	}

	interface IMainViewModel : IViewModelBase, ILoadCallback
	{
		ICollection<IColumnViewModel> Columns { get; }


		ICommand NewTweetCommand { get; }
		ICommand SettingsCommand { get; }
	}
}