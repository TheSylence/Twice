using LinqToTwitter;
using System;
using System.Collections.Generic;
using Twice.Models.Scheduling;
using Twice.Models.Twitter.Entities;

namespace Twice.ViewModels.Twitter
{
	internal class ScheduleItem : ColumnItem
	{
		public ScheduleItem( SchedulerJob job, UserViewModel user )
		{
			Job = job;
			User = user;
			Entities = new Entities
			{
				HashTagEntities = EntityParser.ExtractHashtags( job.Text ),
				MediaEntities = new List<MediaEntity>(),
				SymbolEntities = new List<SymbolEntity>(),
				UrlEntities = new List<UrlEntity>(),
				UserMentionEntities = EntityParser.ExtractMentions( job.Text )
			};
		}

		public override DateTime CreatedAt => Job.TargetTime;
		public override Entities Entities { get; }
		public override ulong Id => Job.JobId;
		public DateTime TargetDate => Job.TargetTime;
		public override string Text => Job.Text;
		public SchedulerJobType Type => Job.JobType;
		private readonly SchedulerJob Job;
	}
}