using System.Collections.Generic;
using System.Globalization;

namespace Twice.ViewModels.Settings
{
	interface IGeneralSettings : ISettingsSection
	{
		ICollection<CultureInfo> AvailableLanguages { get; }
		CultureInfo SelectedLanguage { get; }
		bool RealtimeStreaming { get; set; }
		bool CheckForUpdates { get; set; }
		bool IncludePrereleaseUpdates { get; set; }
	}
}