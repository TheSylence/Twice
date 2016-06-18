namespace Twice.Models.Configuration
{
	internal class GeneralConfig
	{
		public GeneralConfig()
		{
			Language = "en-US";
			RealtimeStreaming = true;
			CheckForUpdates = true;
			IncludePrereleaseUpdates = false;
			TweetFetchCount = 50;
			ColumnsLocked = false;
		}

		public bool CheckForUpdates { get; set; }
		public bool ColumnsLocked { get; set; }
		public bool IncludePrereleaseUpdates { get; set; }
		public string Language { get; set; }
		public bool RealtimeStreaming { get; set; }
		public int TweetFetchCount { get; set; }
	}
}