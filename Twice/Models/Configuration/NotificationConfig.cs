namespace Twice.Models.Configuration
{
	internal class NotificationConfig
	{
		public NotificationConfig()
		{
			ToastsEnabled = true;
			ToastsTop = true;
			ToastsCloseTime = 5;
			SoundEnabled = false;
			PopupEnabled = false;
			PopupDisplay = string.Empty;
			PopupDisplayCorner = Corner.BottomRight;
		}

		public string PopupDisplay { get; set; }
		public Corner PopupDisplayCorner { get; set; }
		public bool PopupEnabled { get; set; }
		public bool SoundEnabled { get; set; }
		public string SoundFileName { get; set; }
		public int ToastsCloseTime { get; set; }
		public bool ToastsEnabled { get; set; }
		public bool ToastsTop { get; set; }
		public bool Win10Enabled { get; set; }
	}
}