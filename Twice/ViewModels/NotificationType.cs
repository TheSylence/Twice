using System;

namespace Twice.ViewModels
{
	[Flags]
	internal enum NotificationType
	{
		Information = 1,
		Success = 2,
		Error = 4,

		Restart = 8
	}
}