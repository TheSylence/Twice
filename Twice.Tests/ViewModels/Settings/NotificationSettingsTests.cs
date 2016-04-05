using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using Twice.Models.Configuration;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass]
	public class NotificationSettingsTests
	{
		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SavedValuesAreAppliedDuringConstruction()
		{
			// Arrange
			var notifi = new NotificationConfig
			{
				PopupEnabled = true,
				SoundEnabled = true,
				ToastsEnabled = true
			};

			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Notifications ).Returns( notifi );

			// Act
			var vm = new NotificationSettings( cfg.Object );

			// Assert
			var toast = vm.EnabledNotifications.OfType<ToastNotificationSettings>().SingleOrDefault();
			Assert.IsNotNull( toast );

			var sound = vm.EnabledNotifications.OfType<SoundNotificationSettings>().SingleOrDefault();
			Assert.IsNotNull( sound );

			var popup = vm.EnabledNotifications.OfType<PopupNotificationSettings>().SingleOrDefault();
			Assert.IsNotNull( popup );
		}
	}
}