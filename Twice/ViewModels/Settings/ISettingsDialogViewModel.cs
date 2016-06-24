using Twice.ViewModels.Main;

namespace Twice.ViewModels.Settings
{
	internal interface ISettingsDialogViewModel : IDialogViewModel, ILoadCallback
	{
		IGeneralSettings General { get; }
		IMuteSettings Mute { get; }
		INotificationSettings Notifications { get; }
		IVisualSettings Visual { get; }
	}
}