using System.Collections.Generic;
using System.Windows.Input;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Main
{
	internal interface IMainViewModel : IViewModelBase, ILoadCallback
	{
		bool HasContexts { get; }
		ICommand AccountsCommand { get; }
		ICollection<IColumnViewModel> Columns { get; }
		ICommand InfoCommand { get; }
		ICommand AddColumnCommand { get; }
		ICommand NewTweetCommand { get; }
		ICommand SettingsCommand { get; }
	}
}