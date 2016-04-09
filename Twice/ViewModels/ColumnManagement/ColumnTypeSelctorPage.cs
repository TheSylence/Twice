using System.Collections.Generic;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class ColumnTypeSelctorPage : WizardPageViewModel
	{
		public ColumnTypeSelctorPage()
		{
			ColumnTypes = new List<ColumnTypeItem>( ColumnTypeListFactory.GetItems() );
		}

		public ICollection<ColumnTypeItem> ColumnTypes { get; }
	}
}