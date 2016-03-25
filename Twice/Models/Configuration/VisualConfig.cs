namespace Twice.Models.Configuration
{
	internal class VisualConfig
	{
		public VisualConfig()
		{
			ThemeName = "BaseLight";
			AccentName = "Blue";
			FontSize = 16;
			HashtagColor = "Red";
			MentionColor = "Green";
			LinkColor = "Blue";
			InlineMedia = true;
			UseStars = true;
		}

		public string AccentName { get; set; }
		public int FontSize { get; set; }
		public string HashtagColor { get; set; }
		public bool InlineMedia { get; set; }
		public string LinkColor { get; set; }
		public string MentionColor { get; set; }
		public string ThemeName { get; set; }
		public bool UseStars { get; set; }
	}
}