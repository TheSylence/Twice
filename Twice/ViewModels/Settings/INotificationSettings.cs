using System.Collections.Generic;
using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	interface INotificationSettings : ISettingsSection
	{
		ICollection<NotificationModuleSettings> AvailableNotifications { get; }
		ICollection<NotificationModuleSettings> EnabledNotifications { get; }
	}
}