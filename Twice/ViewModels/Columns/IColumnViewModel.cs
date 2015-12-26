using System.Collections.Generic;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnViewModel
	{
		Icon Icon { get; }
		ICollection<StatusViewModel> Statuses { get; }
		string Title { get; }
	}
}