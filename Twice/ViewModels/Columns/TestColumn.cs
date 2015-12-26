using System.Collections.Generic;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class TestColumn : IColumnViewModel
	{
		public TestColumn()
		{
			Statuses = new List<StatusViewModel>();
		}

		public string Title => "Test";
		public ICollection<StatusViewModel> Statuses { get; }
	}
}