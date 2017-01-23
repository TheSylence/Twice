namespace Twice.Models.Configuration
{
	internal class GeneralConfig
	{
		public GeneralConfig()
		{
			Language = null;
			RealtimeStreaming = true;
			CheckForUpdates = true;
			IncludePrereleaseUpdates = false;
			TweetFetchCount = 50;
			ColumnsLocked = false;
			FilterSensitiveTweets = false;
			SendVersionStats = true;
		}

		public bool CheckForUpdates { get; set; }
		public bool ColumnsLocked { get; set; }
		public bool FilterSensitiveTweets { get; set; }
		public bool IncludePrereleaseUpdates { get; set; }
		public string Language { get; set; }
		public bool RealtimeStreaming { get; set; }
		public bool SendVersionStats { get; set; }
		public int TweetFetchCount { get; set; }
	}
}