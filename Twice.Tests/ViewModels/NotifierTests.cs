using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Messages;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.ViewModels;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels
{
	[TestClass]
	public class NotifierTests
	{
		[TestMethod, TestCategory( "ViewModels" )]
		public void MessageIsDisplayedWhenToastsAreEnabled()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Notifications ).Returns( new NotificationConfig {ToastsEnabled = true} );
			var messenger = new Mock<IMessenger>();
			messenger.Setup( m => m.Send( It.IsAny<FlyoutMessage>() ) ).Verifiable();

			var notifier = new Notifier( config.Object, messenger.Object, new SyncDispatcher() );

			// Act
			notifier.DisplayMessage( "test", NotificationType.Information );

			// Assert
			messenger.Verify( m => m.Send( It.IsAny<FlyoutMessage>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void MessageIsNotDisplayedWhenToastsAreDisabled()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Notifications ).Returns( new NotificationConfig {ToastsEnabled = false} );
			var messenger = new Mock<IMessenger>();
			messenger.Setup( m => m.Send( It.IsAny<FlyoutMessage>() ) ).Verifiable();

			var notifier = new Notifier( config.Object, messenger.Object, new SyncDispatcher() );

			// Act
			notifier.DisplayMessage( "test", NotificationType.Information );

			// Assert
			messenger.Verify( m => m.Send( It.IsAny<FlyoutMessage>() ), Times.Never() );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void ToastsAreNotRaisedWhenDisabledInColumn()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Notifications ).Returns( new NotificationConfig {ToastsEnabled = true} );
			var messenger = new Mock<IMessenger>();
			messenger.Setup( m => m.Send( It.IsAny<FlyoutMessage>() ) ).Verifiable();

			var status = new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object );
			var notifier = new Notifier( config.Object, messenger.Object, new SyncDispatcher() );

			// Act
			notifier.OnStatus( status, new ColumnNotifications {Toast = false} );

			// Assert
			messenger.Verify( m => m.Send( It.IsAny<FlyoutMessage>() ), Times.Never() );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void ToastsAreNotRaisedWhenDisabledInConfig()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Notifications ).Returns( new NotificationConfig {ToastsEnabled = false} );
			var messenger = new Mock<IMessenger>();
			messenger.Setup( m => m.Send( It.IsAny<FlyoutMessage>() ) ).Verifiable();

			var status = new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object );
			var notifier = new Notifier( config.Object, messenger.Object, new SyncDispatcher() );

			// Act
			notifier.OnStatus( status, new ColumnNotifications {Toast = true} );

			// Assert
			messenger.Verify( m => m.Send( It.IsAny<FlyoutMessage>() ), Times.Never() );
		}

		[TestMethod, TestCategory( "ViewModels" )]
		public void ToastsAreRaisedWhenEnabledInConfig()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Notifications ).Returns( new NotificationConfig {ToastsEnabled = true} );
			var messenger = new Mock<IMessenger>();
			messenger.Setup( m => m.Send( It.IsAny<FlyoutMessage>() ) ).Verifiable();

			var status = new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object );
			var notifier = new Notifier( config.Object, messenger.Object, new SyncDispatcher() );

			// Act
			notifier.OnStatus( status, new ColumnNotifications {Toast = true} );

			// Assert
			messenger.Verify( m => m.Send( It.IsAny<FlyoutMessage>() ), Times.Once() );
		}
	}
}