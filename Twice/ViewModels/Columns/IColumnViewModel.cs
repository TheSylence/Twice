using System.Collections.Generic;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnViewModel
	{
		string Title { get; }

		ICollection<StatusViewModel> Statuses { get; }
	}
}