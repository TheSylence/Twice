using Twice.Models.Configuration;
using Twice.Resources;

namespace Twice.ViewModels.Settings
{
	internal class ToastNotificationSettings : NotificationModuleSettings
	{
		public ToastNotificationSettings( IConfig config, INotifier notifier )
		{
			Enabled = config.Notifications.ToastsEnabled;
			Notifier = notifier;
		}

		public override void SaveTo( IConfig config )
		{
			config.Notifications.ToastsEnabled = Enabled;
		}

		protected override void ExecutePreviewCommand()
		{
			Notifier.DisplayMessage( Strings.TestNotification, NotificationType.Information );
		}

		public override string Title => Strings.InAppNotification;
		private readonly INotifier Notifier;
	}
}