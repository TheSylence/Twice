using System.Collections.Generic;
using LinqToTwitter;
using Twice.ViewModels.Main;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Profile
{
	internal interface IProfileDialogViewModel : IDialogViewModel, ILoadCallback
	{
		void Setup( ulong id );

		Friendship Friendship { get; }
		bool IsBusy { get; }
		UserViewModel User { get; }
		ICollection<UserSubPage> UserPages { get; }
	}
}