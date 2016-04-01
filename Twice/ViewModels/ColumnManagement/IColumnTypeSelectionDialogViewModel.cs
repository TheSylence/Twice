using System.Collections.Generic;

namespace Twice.ViewModels.ColumnManagement
{
	internal interface IColumnTypeSelectionDialogViewModel : IDialogViewModel
	{
		ICollection<ItemSelection<ColumnTypeItem>> AvailableColumnTypes { get; }
		bool SelectAll { get; set; }
	}
}