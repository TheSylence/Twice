using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Configuration;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass]
	public class SoundNotificationSettingsTests
	{
		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SavedValuesAreAppliedDuringConstruction()
		{
			// Arrange
			var notify = new NotificationConfig
			{
				SoundEnabled = true,
				SoundFileName = "file.name"
			};
			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Notifications ).Returns( notify );

			// Act
			var vm = new SoundNotificationSettings( cfg.Object );

			// Assert
			Assert.AreEqual( notify.SoundEnabled, vm.Enabled );
			Assert.AreEqual( notify.SoundFileName, vm.SoundFile );
		}
	}
}