using System.Collections.Generic;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Twitter
{
	internal interface IMessageDetailsViewModel : IDialogViewModel, ILoadCallback
	{
		MessageViewModel Message { get; set; }
		ICollection<MessageViewModel> PreviousMessages { get; }
	}
}