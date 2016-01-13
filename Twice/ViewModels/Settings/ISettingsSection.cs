using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	interface ISettingsSection
	{
		void SaveTo( IConfig config );
	}
}