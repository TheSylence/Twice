using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal class UserSelectorPage : WizardPageViewModel
	{
		public override int NextPageKey { get; protected set; } = -1;
	}
}