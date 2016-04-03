using System.Collections.Generic;
using System.Windows.Input;

namespace Twice.ViewModels.Twitter
{
	internal interface IComposeTweetViewModel : IResetable
	{
		ICollection<AccountEntry> Accounts { get; }
		ICommand AttachImageCommand { get; }
		bool IsSending { get; }
		ICommand SendTweetCommand { get; }
		string Text { get; set; }
	}
}