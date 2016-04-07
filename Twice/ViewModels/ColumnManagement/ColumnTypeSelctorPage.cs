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

		public void SetNextPage( int key )
		{
			_NextPageKey = key;
		}

		public override int NextPageKey => _NextPageKey;

		private int _NextPageKey;
	}
}