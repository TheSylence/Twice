namespace Twice.Models.Configuration
{
	internal class NotificationConfig
	{
		public NotificationConfig()
		{
			ToastsEnabled = true;
			SoundEnabled = false;
			PopupEnabled = false;
			PopupDisplay = string.Empty;
			PopupDisplayCorner = Corner.BottomRight;
		}

		public bool ToastsEnabled { get; set; }
		public bool SoundEnabled { get; set; }
		public string SoundFileName { get; set; }
		public Corner PopupDisplayCorner { get; set; }
		public string PopupDisplay { get; set; }
		public bool PopupEnabled { get; set; }
	}
}