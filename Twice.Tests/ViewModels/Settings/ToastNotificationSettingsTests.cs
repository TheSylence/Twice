using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Configuration;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass]
	public class ToastNotificationSettingsTests
	{
		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SavedValuesAreAppliedDuringConstruction()
		{
			// Arrange
			var notify = new NotificationConfig
			{
				ToastsEnabled = true
			};
			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Notifications ).Returns( notify );

			// Act
			var vm = new ToastNotificationSettings( cfg.Object );

			// Assert
			Assert.AreEqual( notify.ToastsEnabled, vm.Enabled );
		}
	}
}