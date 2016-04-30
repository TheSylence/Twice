using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Configuration;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass]
	public class SettingsDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SettingsAreSavedOnOk()
		{
			// Arrange
			var config = new Mock<IConfig>();
			var visual = new Mock<IVisualSettings>();
			var general = new Mock<IGeneralSettings>();
			var mute = new Mock<IMuteSettings>();
			var notifications = new Mock<INotificationSettings>();

			visual.Setup( x => x.SaveTo( It.IsAny<IConfig>() ) ).Verifiable();
			general.Setup( x => x.SaveTo( It.IsAny<IConfig>() ) ).Verifiable();
			mute.Setup( x => x.SaveTo( It.IsAny<IConfig>() ) ).Verifiable();
			notifications.Setup( x => x.SaveTo( It.IsAny<IConfig>() ) ).Verifiable();

			var vm = new SettingsDialogViewModel( config.Object, visual.Object, general.Object, mute.Object, notifications.Object )
			{
				Dispatcher = new SyncDispatcher()
			};

			// Act
			vm.OkCommand.Execute( null );

			// Assert
			visual.Verify( x => x.SaveTo( It.IsAny<IConfig>() ), Times.Once() );
			general.Verify( x => x.SaveTo( It.IsAny<IConfig>() ), Times.Once() );
			mute.Verify( x => x.SaveTo( It.IsAny<IConfig>() ), Times.Once() );
			notifications.Verify( x => x.SaveTo( It.IsAny<IConfig>() ), Times.Once() );
		}
	}
}