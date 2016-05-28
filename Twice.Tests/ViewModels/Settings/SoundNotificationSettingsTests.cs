using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Configuration;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass, ExcludeFromCodeCoverage]
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

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SaveWritesToConfig()
		{
			// Arrange
			var notify = new NotificationConfig
			{
				SoundEnabled = true,
				SoundFileName = "file.name"
			};
			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Notifications ).Returns( notify );

			var vm = new SoundNotificationSettings( cfg.Object )
			{
				Enabled = false,
				SoundFile = "test"
			};

			// Act
			vm.SaveTo( cfg.Object );

			// Assert
			Assert.AreEqual( false, notify.SoundEnabled );
			Assert.AreEqual( "test", notify.SoundFileName );
		}
	}
}