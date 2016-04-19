using System.Collections.Generic;

namespace Twice.ViewModels.Settings
{
	internal interface INotificationSettings : ISettingsSection
	{
		ICollection<NotificationModuleSettings> AvailableNotifications { get; }
		ICollection<NotificationModuleSettings> EnabledNotifications { get; }
	}
}