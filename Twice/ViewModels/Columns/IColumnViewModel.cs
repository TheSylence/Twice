using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twice.Models.Columns;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnViewModel
	{
		event EventHandler<StatusEventArgs> NewStatus;

		event EventHandler Resized;

		Task Load();

		IColumnActionDispatcher ActionDispatcher { get; }
		ColumnDefinition Definition { get; }
		Icon Icon { get; }
		bool IsLoading { get; }
		ICollection<StatusViewModel> Statuses { get; }
		string Title { get; set; }
		double Width { get; set; }
	}
}