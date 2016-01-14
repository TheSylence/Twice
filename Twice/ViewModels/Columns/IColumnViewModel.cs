using System.Collections.Generic;
using System.Threading.Tasks;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnViewModel
	{
		Task Load();

		Icon Icon { get; }
		bool IsLoading { get; }
		ICollection<StatusViewModel> Statuses { get; }
		string Title { get; }
		double Width { get; set; }
	}
}