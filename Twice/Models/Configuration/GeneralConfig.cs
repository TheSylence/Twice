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
		}

		public string Language { get; set; }
		public bool RealtimeStreaming { get; set; }
		public bool CheckForUpdates { get; set; }
		public bool IncludePrereleaseUpdates { get; set; }
	}
}