namespace Twice.ViewModels.Settings
{
	internal class SettingsDialogViewModel : DialogViewModel, ISettingsDialogViewModel
	{
		public SettingsDialogViewModel( IVisualSettings visualSettings )
		{
			Visual = visualSettings;
		}

		public IVisualSettings Visual { get; }
	}
}