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

		protected override bool CanExecutePreviewCommand()
		{
			return true;
		}

		protected override void ExecutePreviewCommand()
		{
			Notifier.DisplayMessage( "This is a test notification", NotificationType.Information );
		}

		public override string Title => Strings.InAppNotification;
		private readonly INotifier Notifier;
	}
}