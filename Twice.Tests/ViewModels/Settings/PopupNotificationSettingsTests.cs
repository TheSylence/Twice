using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Configuration;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass]
	public class PopupNotificationSettingsTests
	{
		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SavedValuesAreAppliedDuringConstruction()
		{
			// Arrange
			var notify = new NotificationConfig
			{
				PopupEnabled = true,
				PopupDisplayCorner = Corner.TopLeft,
				PopupDisplay = "TestDisplay"
			};
			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Notifications ).Returns( notify );

			// Act
			var vm = new PopupNotificationSettings( cfg.Object );

			// Assert
			Assert.AreEqual( notify.PopupEnabled, vm.Enabled );
			Assert.AreEqual( notify.PopupDisplayCorner, vm.SelectedCorner );
			Assert.AreEqual( notify.PopupDisplay, vm.SelectedDisplay );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void ValuesAreCorrectlySaved()
		{
			// Arrange
			var notify = new NotificationConfig
			{
				PopupEnabled = true,
				PopupDisplayCorner = Corner.TopLeft,
				PopupDisplay = "TestDisplay"
			};
			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Notifications ).Returns( notify );

			// Act
			var vm = new PopupNotificationSettings( cfg.Object )
			{
				SelectedCorner = Corner.BottomRight,
				SelectedDisplay = "test",
				Enabled = false
			};

			vm.SaveTo( cfg.Object );

			// Assert
			Assert.AreEqual( vm.SelectedDisplay, notify.PopupDisplay );
			Assert.AreEqual( vm.SelectedCorner, notify.PopupDisplayCorner );
			Assert.AreEqual( vm.Enabled, notify.PopupEnabled );
		}
	}
}