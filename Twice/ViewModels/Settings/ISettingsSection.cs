using Twice.Models.Configuration;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Settings
{
	internal interface ISettingsSection : ILoadCallback
	{
		void SaveTo( IConfig config );
	}
}