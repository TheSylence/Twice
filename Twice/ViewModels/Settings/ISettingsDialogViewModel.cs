namespace Twice.ViewModels.Settings
{
	internal interface ISettingsDialogViewModel : IDialogViewModel
	{
		IGeneralSettings General { get; }
		IVisualSettings Visual { get; }
		IMuteSettings Mute { get; }
	}
}