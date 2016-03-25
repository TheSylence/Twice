using LinqToTwitter;
using System.Collections.Generic;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Profile
{
	internal interface IProfileDialogViewModel : IDialogViewModel, ILoadCallback
	{
		void Setup( ulong id );

		Friendship Friendship { get; }
		bool IsBusy { get; }
		User User { get; }
		ICollection<UserSubPage> UserPages { get; }
	}
}