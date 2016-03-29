namespace Twice.Models.Configuration
{
	internal class VisualConfig
	{
		public VisualConfig()
		{
			UseDarkTheme = true;
			PrimaryColor = "bluegrey";
			AccentColor = "blue";
			FontSize = 16;
			HashtagColor = "blue";
			MentionColor = "blue";
			LinkColor = "blue";
			InlineMedia = true;
		}

		public string AccentColor { get; set; }
		public int FontSize { get; set; }
		public string HashtagColor { get; set; }
		public bool InlineMedia { get; set; }
		public string LinkColor { get; set; }
		public string MentionColor { get; set; }
		public string PrimaryColor { get; set; }
		public bool UseDarkTheme { get; set; }
	}
}