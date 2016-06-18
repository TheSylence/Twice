using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Models.Configuration;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass, ExcludeFromCodeCoverage]
	public class NotificationSettingsTests
	{
		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void DisablingNotificationRemovesFromCollection()
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

			var vm = new NotificationSettings( cfg.Object, null );

			// Act
			vm.AvailableNotifications.First().Enabled = false;

			// Assert
			Assert.AreEqual( 2, vm.EnabledNotifications.Count );
			CollectionAssert.DoesNotContain( vm.EnabledNotifications.ToArray(), vm.AvailableNotifications.First() );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void EnablingNotificationAddsToCollection()
		{
			// Arrange
			var notifi = new NotificationConfig
			{
				PopupEnabled = false,
				SoundEnabled = false,
				ToastsEnabled = false
			};

			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Notifications ).Returns( notifi );

			var vm = new NotificationSettings( cfg.Object, null );

			// Act
			vm.AvailableNotifications.First().Enabled = true;

			// Assert
			Assert.AreEqual( 1, vm.EnabledNotifications.Count );
			CollectionAssert.Contains( vm.EnabledNotifications.ToArray(), vm.AvailableNotifications.First() );
		}

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
			var vm = new NotificationSettings( cfg.Object, null );

			// Assert
			var toast = vm.EnabledNotifications.OfType<ToastNotificationSettings>().SingleOrDefault();
			Assert.IsNotNull( toast );

			var sound = vm.EnabledNotifications.OfType<SoundNotificationSettings>().SingleOrDefault();
			Assert.IsNotNull( sound );

			var popup = vm.EnabledNotifications.OfType<PopupNotificationSettings>().SingleOrDefault();
			Assert.IsNotNull( popup );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SaveWritesModuleSettingsToConfig()
		{
			// Arrange
			var notifi = new NotificationConfig
			{
				PopupEnabled = true,
				SoundEnabled = false,
				ToastsEnabled = false
			};

			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Notifications ).Returns( notifi );

			var vm = new NotificationSettings( cfg.Object, null );

			// Act
			vm.AvailableNotifications.First( n => n.Enabled ).Enabled = false;
			vm.SaveTo( cfg.Object );

			// Assert
			Assert.AreEqual( false, notifi.PopupEnabled );
		}
	}
}