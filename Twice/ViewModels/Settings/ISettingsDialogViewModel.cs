namespace Twice.ViewModels.Settings
{
	internal interface ISettingsDialogViewModel : IDialogViewModel
	{
		IGeneralSettings General { get; }
		IMuteSettings Mute { get; }
		INotificationSettings Notifications { get; }
		IVisualSettings Visual { get; }
	}
}