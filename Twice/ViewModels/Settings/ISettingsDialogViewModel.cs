namespace Twice.ViewModels.Settings
{
	internal interface ISettingsDialogViewModel : IDialogViewModel
	{
		IVisualSettings Visual { get; }
	}
}