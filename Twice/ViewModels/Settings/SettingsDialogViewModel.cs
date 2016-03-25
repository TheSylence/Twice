using Twice.Models.Configuration;
using Twice.Resources;

namespace Twice.ViewModels.Settings
{
	internal class SettingsDialogViewModel : DialogViewModel, ISettingsDialogViewModel
	{
		public SettingsDialogViewModel( IConfig config, IVisualSettings visual, IGeneralSettings general,
			IMuteSettings mute, INotificationSettings notifications )
		{
			Config = config;
			Visual = visual;
			General = general;
			Mute = mute;
			Notifications = notifications;

			Title = Strings.Settings;
		}

		protected override bool OnOk()
		{
			General.SaveTo( Config );
			Visual.SaveTo( Config );
			Mute.SaveTo( Config );
			Notifications.SaveTo( Config );
			Config.Save();

			return base.OnOk();
		}

		public IGeneralSettings General { get; }
		public IMuteSettings Mute { get; }
		public INotificationSettings Notifications { get; }
		public IVisualSettings Visual { get; }
		private readonly IConfig Config;
	}
}