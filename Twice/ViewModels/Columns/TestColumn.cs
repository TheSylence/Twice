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

		public Icon Icon => ViewModels.Icon.User;

		public string Title => "Test";
		public ICollection<StatusViewModel> Statuses { get; }
	}
}