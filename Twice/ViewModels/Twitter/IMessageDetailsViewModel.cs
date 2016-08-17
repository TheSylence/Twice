using System.Collections.Generic;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Twitter
{
	internal interface IMessageDetailsViewModel : IDialogViewModel, ILoadCallback, IScrollController
	{
		bool IsLoadingPrevious { get; }
		MessageViewModel Message { get; set; }
		ICollection<MessageViewModel> PreviousMessages { get; }
	}
}