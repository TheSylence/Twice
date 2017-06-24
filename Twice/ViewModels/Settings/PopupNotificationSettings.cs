using System.Collections.Generic;
using System.Linq;
using Twice.Models.Configuration;
using Twice.Resources;
using Twice.Utilities.Os;

namespace Twice.ViewModels.Settings
{
	internal class PopupNotificationSettings : NotificationModuleSettings
	{
		public PopupNotificationSettings( IConfig config, INotifier notifier )
		{
			Notifier = notifier;
			AvailableCorners = ValueDescription<Corner>.GetValues( true ).ToList();
			AvailableDisplays = ListDisplays().ToList();

			Enabled = config.Notifications.PopupEnabled;
			SelectedCorner = config.Notifications.PopupDisplayCorner;
			SelectedDisplay = config.Notifications.PopupDisplay;
			Win10Enabled = config.Notifications.Win10Enabled;
			CloseTime = config.Notifications.ToastsCloseTime;
		}

		public override void SaveTo( IConfig config )
		{
			config.Notifications.PopupEnabled = Enabled;
			config.Notifications.PopupDisplayCorner = SelectedCorner;
			config.Notifications.PopupDisplay = SelectedDisplay;
			config.Notifications.Win10Enabled = Win10Enabled;
			config.Notifications.PopupCloseTime = CloseTime;
		}

		protected override void ExecutePreviewCommand()
		{
			Notifier.PreviewPopupNotification( Strings.TestNotification, CloseTime, Win10Enabled, SelectedDisplay, SelectedCorner );
		}

		private static IEnumerable<ValueDescription<string>> ListDisplays()
		{
			return DisplayHelper.GetAvailableDisplays().Select( kvp => new ValueDescription<string>( kvp.Key, kvp.Value ) );
		}

		public ICollection<ValueDescription<Corner>> AvailableCorners { get; }

		public ICollection<ValueDescription<string>> AvailableDisplays { get; }

		public int CloseTime { get; set; }

		public Corner SelectedCorner { get; set; }

		public string SelectedDisplay { get; set; }

		public override string Title => Strings.PopupNotification;

		public bool Win10Enabled { get; set; }

		private readonly INotifier Notifier;
	}
}