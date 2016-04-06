using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twice.Models.Columns;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnViewModel
	{
		event EventHandler Changed;

		event EventHandler<StatusEventArgs> NewStatus;

		Task Load();

		IColumnActionDispatcher ActionDispatcher { get; }
		IColumnConfigurationViewModel ColumnConfiguration { get; }
		ColumnDefinition Definition { get; }
		Icon Icon { get; }
		bool IsLoading { get; }
		ICollection<StatusViewModel> Statuses { get; }
		string Title { get; set; }
		double Width { get; set; }
	}
}