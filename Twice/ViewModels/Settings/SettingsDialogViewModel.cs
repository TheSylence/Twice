using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	internal class SettingsDialogViewModel : DialogViewModel, ISettingsDialogViewModel
	{
		public SettingsDialogViewModel( IConfig config, IVisualSettings visual, IGeneralSettings general, IMuteSettings mute )
		{
			Config = config;
			Visual = visual;
			General = general;
			Mute = mute;
		}

		protected override bool OnOk()
		{
			General.SaveTo( Config );
			Visual.SaveTo( Config );
			Mute.SaveTo( Config );
			Config.Save();

			return base.OnOk();
		}

		public IMuteSettings Mute { get; }
		public IGeneralSettings General { get; }
		public IVisualSettings Visual { get; }
		private readonly IConfig Config;
	}
}