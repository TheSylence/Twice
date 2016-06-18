using System.Collections.Generic;
using System.Windows.Input;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Main
{
	internal interface IMainViewModel : IViewModelBase, ILoadCallback
	{
		ICommand AccountsCommand { get; }
		ICommand AddColumnCommand { get; }
		ICollection<IColumnViewModel> Columns { get; }
		bool ColumnsLocked { get; }
		IDragDropHandler DragDropHandler { get; }
		bool HasContexts { get; }
		ICommand InfoCommand { get; }
		ICommand NewMessageCommand { get; }
		ICommand NewTweetCommand { get; }
		ICommand SearchCommand { get; }
		ICommand SettingsCommand { get; }
		ICommand ToggleColumnsLockCommand { get; }
	}
}