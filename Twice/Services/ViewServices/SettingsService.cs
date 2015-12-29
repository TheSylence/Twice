using Twice.ViewModels.Settings;
using Twice.Views;

namespace Twice.Services.ViewServices
{
	internal interface ISettingsService : IViewService
	{ }

	internal class SettingsService : ViewServiceBase<SettingsDialog, SettingsDialogViewModel, object>, ISettingsService
	{
	}
}