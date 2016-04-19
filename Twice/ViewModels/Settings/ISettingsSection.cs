using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	internal interface ISettingsSection
	{
		void SaveTo( IConfig config );
	}
}