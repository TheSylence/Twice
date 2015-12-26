using System.Collections.Generic;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Main
{
	interface IMainViewModel : IViewModelBase
	{
		ICollection<IColumnViewModel> Columns { get; }
	}
}