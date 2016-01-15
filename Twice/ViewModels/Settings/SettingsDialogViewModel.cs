using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	internal class SettingsDialogViewModel : DialogViewModel, ISettingsDialogViewModel
	{
		public SettingsDialogViewModel( IConfig config, IVisualSettings visual, IGeneralSettings general )
		{
			Config = config;
			Visual = visual;
			General = general;
		}

		protected override bool OnOk()
		{
			General.SaveTo( Config );
			Visual.SaveTo( Config );
			Config.Save();

			return base.OnOk();
		}

		public IGeneralSettings General { get; }
		public IVisualSettings Visual { get; }
		private readonly IConfig Config;
	}
}