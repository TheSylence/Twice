using System.Collections.Generic;
using System.Globalization;

namespace Twice.ViewModels.Settings
{
	internal interface IGeneralSettings : ISettingsSection
	{
		ICollection<CultureInfo> AvailableLanguages { get; }
		bool CheckForUpdates { get; set; }
		bool IncludePrereleaseUpdates { get; set; }
		bool RealtimeStreaming { get; set; }
		CultureInfo SelectedLanguage { get; }
	}
}