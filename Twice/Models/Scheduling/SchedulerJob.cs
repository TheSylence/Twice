using System;
using System.Collections.Generic;

namespace Twice.Models.Scheduling
{
	internal class SchedulerJob
	{
		public ulong AccountId { get; set; }
		public List<string> FilesToAttach { get; set; }
		public ulong IdToDelete { get; set; }
		public ulong InReplyToStatus { get; set; }
		public string InReplyToUser { get; set; }
		public SchedulerJobType JobType { get; set; }
		public DateTime TargetTime { get; set; }
		public string Text { get; set; }
	}
}