using Twice.ViewModels.Main;

namespace Twice.ViewModels.Twitter
{
	internal interface IComposeMessageViewModel : IDialogViewModel, ILoadCallback
	{
		bool? CanSend { get; }
		MessageViewModel InReplyTo { get; set; }
		bool IsCheckingRelationship { get; }
		bool IsSending { get; }
		string Recipient { get; set; }
		string Text { get; set; }
	}
}