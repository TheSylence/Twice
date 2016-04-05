using System.Collections.Generic;
using System.Windows.Input;

namespace Twice.ViewModels.Twitter
{
	internal interface IComposeTweetViewModel : IResetable
	{
		ICollection<AccountEntry> Accounts { get; }
		ICollection<MediaItem> AttachedMedias { get; }
		ICommand AttachImageCommand { get; }
		bool ConfirmationRequired { get; }
		bool ConfirmationSet { get; set; }
		bool IsSending { get; }
		ICollection<string> KnownHashtags { get; }
		ICollection<string> KnownUserNames { get; }
		bool LowCharsLeft { get; set; }
		bool MediumCharsLeft { get; set; }
		ICommand SendTweetCommand { get; }
		bool StayOpen { get; set; }
		string Text { get; set; }
		int TextLength { get; set; }
	}
}