using System;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class StatusEventArgs : EventArgs
	{
		public StatusEventArgs( StatusViewModel status )
		{
			Status = status;
		}

		public readonly StatusViewModel Status;
	}
}