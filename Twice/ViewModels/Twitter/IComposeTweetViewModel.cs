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
		int TextLength { get; set; }
		bool MediumCharsLeft { [System.Diagnostics.DebuggerStepThrough] get; set; }
		bool LowCharsLeft { [System.Diagnostics.DebuggerStepThrough] get; set; }
		ICollection<MediaItem> AttachedMedias { get; }
	}
}