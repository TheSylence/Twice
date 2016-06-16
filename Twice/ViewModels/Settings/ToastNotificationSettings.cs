using Twice.Models.Configuration;
using Twice.Resources;

namespace Twice.ViewModels.Settings
{
	internal class ToastNotificationSettings : NotificationModuleSettings
	{
		public ToastNotificationSettings( IConfig config )
		{
			Enabled = config.Notifications.ToastsEnabled;
		}

		public override void SaveTo( IConfig config )
		{
			config.Notifications.ToastsEnabled = Enabled;
		}

		public override string Title => Strings.InAppNotification;
	}
}