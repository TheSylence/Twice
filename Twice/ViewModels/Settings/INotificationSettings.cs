using System.Collections.Generic;
using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	interface INotificationSettings : ISettingsSection
	{
		ICollection<NotificationModuleSettings> AvailableNotifications { get; }
		Corner DisplayCorner { get; set; }
		int DisplayIndex { get; set; }
		ICollection<NotificationModuleSettings> EnabledNotifications { get; }
		bool EnablePopups { get; set; }
		bool EnableSounds { get; set; }
		bool EnableToasts { get; set; }
		string SoundFileName { get; set; }
	}
}