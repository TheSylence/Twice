using System;

namespace Twice.ViewModels.Settings
{
	internal class MuteEditArgs : EventArgs
	{
		public MuteEditArgs( MuteEditAction action, string filter, DateTime? endDate )
		{
			Action = action;
			Filter = filter;
			EndDate = endDate;
		}

		public readonly MuteEditAction Action;
		public readonly DateTime? EndDate;
		public readonly string Filter;
	}
}