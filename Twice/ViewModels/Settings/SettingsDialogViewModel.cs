using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	internal class SettingsDialogViewModel : DialogViewModel, ISettingsDialogViewModel
	{
		public SettingsDialogViewModel( IConfig config, IVisualSettings visualSettings )
		{
			Config = config;
			Visual = visualSettings;
		}

		protected override bool OnOk()
		{
			Visual.SaveTo( Config );

			return base.OnOk();
		}

		public IVisualSettings Visual { get; }
		private readonly IConfig Config;
	}
}