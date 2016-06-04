using System.Collections.Generic;
using Twice.Models.Twitter;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Twitter
{
	internal interface ITweetDetailsViewModel : IDialogViewModel, ILoadCallback
	{
		IContextEntry Context { get; set; }
		StatusViewModel DisplayTweet { get; set; }
		IList<StatusViewModel> FollowingConversationTweets { get; }
		bool IsLoadingFollowing { get; }
		bool IsLoadingPrevious { get; }
		IList<StatusViewModel> PreviousConversationTweets { get; }
	}
}