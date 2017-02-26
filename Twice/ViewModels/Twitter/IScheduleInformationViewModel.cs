using System;
using System.Collections.Generic;

namespace Twice.ViewModels.Twitter
{
	interface IScheduleInformationViewModel
	{
		void ResetSchedule();
		void ScheduleDeletion( List<Tuple<ulong, ulong>> tweetIds, string text );
		void ScheduleTweet( string text, ulong? inReplyTo, IEnumerable<ulong> accountIds, IEnumerable<string> mediaFileNames );
		DateTime DeletionDate { get; set; }
		DateTime DeletionTime { get; set; }
		bool IsDeletionScheduled { get; set; }
		bool IsTweetScheduled { get; set; }
		DateTime ScheduleDate { get; set; }
		DateTime ScheduleTime { get; set; }
	}
}