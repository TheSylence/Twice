using System.Collections.Generic;
using LinqToTwitter;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Profile
{
	internal interface IProfileDialogViewModel : IDialogViewModel, ILoadCallback
	{
		Friendship Friendship { get; }
		bool IsBusy { get; }
		User User { get; }
		ICollection<UserSubPage> UserPages { get; }
	}
}