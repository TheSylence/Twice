using System.Collections.Generic;
using System.Globalization;

namespace Twice.ViewModels.Settings
{
	internal interface IGeneralSettings : ISettingsSection
	{
		ICollection<int> AvailableFetchCounts { get; }
		ICollection<CultureInfo> AvailableLanguages { get; }
		bool CheckForUpdates { get; set; }
		bool IncludePrereleaseUpdates { get; set; }
		bool RealtimeStreaming { get; set; }
		CultureInfo SelectedLanguage { get; }
		int TweetFetchCount { get; set; }
	}
}