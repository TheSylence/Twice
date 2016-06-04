using System.Collections.Generic;
using System.Windows.Input;
using LinqToTwitter;
using Twice.ViewModels.Main;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Profile
{
	internal interface IProfileDialogViewModel : IDialogViewModel, ILoadCallback
	{
		void Setup( ulong id );

		ICommand FollowUserCommand { get; }
		Friendship Friendship { get; }
		bool IsBusy { get; }
		ICommand UnfollowUserCommand { get; }
		UserViewModel User { get; }
		ICollection<UserSubPage> UserPages { get; }
	}
}