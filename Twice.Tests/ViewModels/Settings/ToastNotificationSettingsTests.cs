using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Configuration;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass, ExcludeFromCodeCoverage]
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

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SaveWritesToConfig()
		{
			// Arrange
			var notify = new NotificationConfig
			{
				ToastsEnabled = true
			};
			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Notifications ).Returns( notify );

			var vm = new ToastNotificationSettings( cfg.Object )
			{
				Enabled = false
			};

			// Act
			vm.SaveTo( cfg.Object );

			// Assert
			Assert.AreEqual( false, notify.ToastsEnabled );
		}
	}
}