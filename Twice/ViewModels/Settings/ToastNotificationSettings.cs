using System.Diagnostics;
using Twice.Models.Configuration;
using Twice.Resources;

namespace Twice.ViewModels.Settings
{
	internal class ToastNotificationSettings : NotificationModuleSettings
	{
		public ToastNotificationSettings( IConfig config, INotifier notifier )
		{
			Notifier = notifier;

			Enabled = config.Notifications.ToastsEnabled;
			Top = config.Notifications.ToastsTop;
			CloseTime = config.Notifications.ToastsCloseTime;
		}

		public override void SaveTo( IConfig config )
		{
			config.Notifications.ToastsEnabled = Enabled;
			config.Notifications.ToastsTop = Top;
			config.Notifications.ToastsCloseTime = CloseTime;
		}

		protected override void ExecutePreviewCommand()
		{
			Notifier.PreviewInAppNotification( Strings.TestNotification, Top, CloseTime );
		}

		public int CloseTime { get; set; }

		public override string Title => Strings.InAppNotification;

		public bool Top { get; set; }

		private readonly INotifier Notifier;
	}
}