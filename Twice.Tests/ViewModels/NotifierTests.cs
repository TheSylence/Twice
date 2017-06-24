using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.ViewModels;
using Twice.ViewModels.Flyouts;
using Twice.ViewModels.Twitter;
using Twice.Views.Services;

namespace Twice.Tests.ViewModels
{
	[TestClass, ExcludeFromCodeCoverage]
	public class NotifierTests
	{
		[TestMethod, TestCategory( "ViewModels" )]
		public void MessageIsDisplayedWhenToastsAreEnabled()
		{
			// Arrange
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Notifications ).Returns( new NotificationConfig { ToastsEnabled = true } );
			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.OpenNotificationFlyout( It.IsAny<NotificationViewModel>() ) ).Verifiable();

			var notifier = new Notifier( config.Object, null, new SyncDispatcher(), viewServices.Object, null );

			// Act
			notifier.DisplayMessage( "test", NotificationType.Information );

			// Assert
			viewServices.Verify( v => v.OpenNotificationFlyout( It.IsAny<NotificationViewModel>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void MessageIsNotDisplayedWhenToastsAreDisabled()
		{
			// Arrange
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Notifications ).Returns( new NotificationConfig { ToastsEnabled = false } );
			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.OpenNotificationFlyout( It.IsAny<NotificationViewModel>() ) ).Verifiable();

			var notifier = new Notifier( config.Object, null, new SyncDispatcher(), viewServices.Object, null);

			// Act
			notifier.DisplayMessage( "test", NotificationType.Information );

			// Assert
			viewServices.Verify( v => v.OpenNotificationFlyout( It.IsAny<NotificationViewModel>() ), Times.Never() );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void ToastsAreNotRaisedWhenDisabledInColumn()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Notifications ).Returns( new NotificationConfig { ToastsEnabled = true } );
			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.OpenNotificationFlyout( It.IsAny<NotificationViewModel>() ) ).Verifiable();

			var notifier = new Notifier( config.Object, null, new SyncDispatcher(), viewServices.Object, null);

			var status = new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null, null );

			// Act
			notifier.OnItem( status, new ColumnNotifications { Toast = false } );

			// Assert
			viewServices.Verify( v => v.OpenNotificationFlyout( It.IsAny<NotificationViewModel>() ), Times.Never() );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void ToastsAreNotRaisedWhenDisabledInConfig()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Notifications ).Returns( new NotificationConfig { ToastsEnabled = false } );
			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.OpenNotificationFlyout( It.IsAny<NotificationViewModel>() ) ).Verifiable();

			var notifier = new Notifier( config.Object, null, new SyncDispatcher(), viewServices.Object, null);

			var status = new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null, null );

			// Act
			notifier.OnItem( status, new ColumnNotifications { Toast = true } );

			// Assert
			viewServices.Verify( v => v.OpenNotificationFlyout( It.IsAny<NotificationViewModel>() ), Times.Never() );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void ToastsAreRaisedWhenEnabledInConfig()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Notifications ).Returns( new NotificationConfig { ToastsEnabled = true } );
			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.OpenNotificationFlyout( It.IsAny<NotificationViewModel>() ) ).Verifiable();

			var notifier = new Notifier( config.Object, null, new SyncDispatcher(), viewServices.Object, null);
			var status = new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null, null );

			// Act
			notifier.OnItem( status, new ColumnNotifications { Toast = true } );

			// Assert
			viewServices.Verify( v => v.OpenNotificationFlyout( It.IsAny<NotificationViewModel>() ), Times.Once() );
		}
	}
}