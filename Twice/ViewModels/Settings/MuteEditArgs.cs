using System;

namespace Twice.ViewModels.Settings
{
	class MuteEditArgs : EventArgs
	{
		public MuteEditArgs( MuteEditAction action, string filter, DateTime? endDate )
		{
			Action = action;
			Filter = filter;
			EndDate = endDate;
		}

		public readonly string Filter;
		public readonly DateTime? EndDate;
		public readonly MuteEditAction Action;
	}
}