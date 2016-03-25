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
			HashtagColor = "red";
			MentionColor = "green";
			LinkColor = "tial";
			InlineMedia = true;
			UseStars = true;
		}

		public string AccentColor { get; set; }
		public int FontSize { get; set; }
		public string HashtagColor { get; set; }
		public bool InlineMedia { get; set; }
		public string LinkColor { get; set; }
		public string MentionColor { get; set; }
		public string PrimaryColor { get; set; }
		public bool UseDarkTheme { get; set; }
		public bool UseStars { get; set; }
	}
}