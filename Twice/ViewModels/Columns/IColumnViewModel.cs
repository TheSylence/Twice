using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Twice.Models.Columns;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnViewModel
	{
		event EventHandler Changed;
		event EventHandler Deleted;
		event EventHandler<StatusEventArgs> NewStatus;

		Task Load();

		IColumnActionDispatcher ActionDispatcher { get; }
		ICommand ClearCommand { get; }
		IColumnConfigurationViewModel ColumnConfiguration { get; }
		ColumnDefinition Definition { get; }
		ICommand DeleteCommand { get; }
		Icon Icon { get; }
		bool IsLoading { get; }
		ICollection<StatusViewModel> Statuses { get; }
		string Title { get; set; }
		double Width { get; set; }
	}
}