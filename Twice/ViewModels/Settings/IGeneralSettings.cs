using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;

namespace Twice.ViewModels.Settings
{
	internal interface IGeneralSettings : ISettingsSection
	{
		ICollection<int> AvailableFetchCounts { get; }
		ICollection<CultureInfo> AvailableLanguages { get; }
		bool CheckForUpdates { get; set; }
		ICommand ClearCacheCommand { get; }
		bool IncludePrereleaseUpdates { get; set; }
		bool RealtimeStreaming { get; set; }
		CultureInfo SelectedLanguage { get; }
		int TweetFetchCount { get; set; }
	}
}