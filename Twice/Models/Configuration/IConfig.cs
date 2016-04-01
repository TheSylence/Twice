namespace Twice.Models.Configuration
{
	internal interface IConfig
	{
		void Save();

		GeneralConfig General { get; }
		MuteConfig Mute { get; }
		NotificationConfig Notifications { get; }
		VisualConfig Visual { get; }
	}
}