using Twice.ViewModels.Profile;
using Twice.Views;

namespace Twice.Services.ViewServices
{
	internal interface IViewProfileService : IViewService
	{ }

	internal class ViewProfileService : ViewServiceBase<ProfileDialog, ProfileDialogViewModel, object>, IViewProfileService
	{
		protected override void SetupViewModel( ProfileDialogViewModel vm, object args )
		{
			base.SetupViewModel( vm, args );

			ulong profileId = (ulong)args;

			vm.Setup( profileId );
		}
	}
}