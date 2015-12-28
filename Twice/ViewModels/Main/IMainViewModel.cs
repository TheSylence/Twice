using System.Collections.Generic;
using System.Windows.Input;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Main
{
	interface IMainViewModel : IViewModelBase
	{
		ICollection<IColumnViewModel> Columns { get; }

		ICommand NewTweetCommand { get; }
		ICommand SettingsCommand { get; }
	}
}