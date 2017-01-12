using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Twitter
{
	internal interface IComposeTweetViewModel : ILoadCallback, IDialogViewModel, ICursorController, IDropTarget
	{
		void PreSelectAccounts( IEnumerable<ulong> accounts );

		void SetInitialText( string text );

		void SetReply( StatusViewModel status, bool toAll );

		ICollection<AccountEntry> Accounts { get; }
		IList<MediaItem> AttachedMedias { get; }
		ICommand AttachImageCommand { get; }
		bool ConfirmationRequired { get; }
		bool ConfirmationSet { get; set; }
		ICommand DeleteMediaCommand { get; }
		DateTime DeletionDate { get; set; }
		DateTime DeletionTime { get; set; }
		StatusViewModel InReplyTo { get; set; }
		bool IsDeletionScheduled { get; set; }
		bool IsSending { get; }
		bool IsTweetScheduled { get; set; }
		ICollection<string> KnownHashtags { get; }
		ICollection<string> KnownUserNames { get; }
		bool LowCharsLeft { get; set; }
		bool MediumCharsLeft { get; set; }
		StatusViewModel QuotedTweet { get; set; }
		ICommand RemoveQuoteCommand { get; }
		ICommand RemoveReplyCommand { get; }
		DateTime ScheduleDate { get; set; }
		DateTime ScheduleTime { get; set; }
		ICommand SendTweetCommand { get; }
		bool StayOpen { get; set; }
		string Text { get; set; }
		int TextLength { get; set; }
	}
}