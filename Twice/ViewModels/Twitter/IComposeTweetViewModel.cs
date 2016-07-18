using System;
using System.Collections.Generic;
using System.Windows.Input;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Twitter
{
	internal interface IComposeTweetViewModel : ILoadCallback, IDialogViewModel
	{
		void PreSelectAccounts( IEnumerable<ulong> accounts );

		void SetReply( StatusViewModel status, bool toAll );

		ICollection<AccountEntry> Accounts { get; }
		IList<MediaItem> AttachedMedias { get; }
		ICommand AttachImageCommand { get; }
		bool ConfirmationRequired { get; }
		bool ConfirmationSet { get; set; }
		ICommand DeleteMediaCommand { get; }
		StatusViewModel InReplyTo { get; set; }
		bool IsSending { get; }
		ICollection<string> KnownHashtags { get; }
		ICollection<string> KnownUserNames { get; }
		bool LowCharsLeft { get; set; }
		bool MediumCharsLeft { get; set; }
		StatusViewModel QuotedTweet { get; set; }
		ICommand RemoveQuoteCommand { get; }
		ICommand RemoveReplyCommand { get; }
		ICommand SendTweetCommand { get; }
		bool StayOpen { get; set; }
		string Text { get; set; }
		int TextLength { get; set; }
		bool IsTweetScheduled { get; set; }
		bool IsDeletionScheduled { get; set; }

		DateTime ScheduleDate { get; set; }
		DateTime DeletionDate { get; set; }
		DateTime ScheduleTime { get; set; }
		DateTime DeletionTime { get; set; }
	}
}