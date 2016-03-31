using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twice.ViewModels.Columns.Definitions;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnViewModel
	{
		event EventHandler<StatusEventArgs> NewStatus;

		Task Load();

		ColumnDefinition Definition { get; }
		Icon Icon { get; }
		bool IsLoading { get; }
		ICollection<StatusViewModel> Statuses { get; }
		string Title { get; set; }
		double Width { get; set; }
	}

	internal class StatusEventArgs : EventArgs
	{
		public StatusEventArgs( StatusViewModel status )
		{
			Status = status;
		}

		public readonly StatusViewModel Status;
	}
}