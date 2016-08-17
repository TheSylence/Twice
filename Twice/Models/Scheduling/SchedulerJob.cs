using System;
using System.Collections.Generic;

namespace Twice.Models.Scheduling
{
	internal class SchedulerJob
	{
		public SchedulerJob()
		{
			AccountIds = new List<ulong>();
			FilesToAttach = new List<string>();
			IdsToDelete = new List<ulong>();
		}

		public override bool Equals( object obj )
		{
			var job = obj as SchedulerJob;
			return JobId == job?.JobId;
		}

		public override int GetHashCode()
		{
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			return JobId.GetHashCode();
		}

		public List<ulong> AccountIds { get; set; }
		public List<string> FilesToAttach { get; set; }
		public List<ulong> IdsToDelete { get; set; }
		public ulong InReplyToStatus { get; set; }
		public ulong JobId { get; set; }
		public SchedulerJobType JobType { get; set; }
		public DateTime TargetTime { get; set; }
		public string Text { get; set; }
	}
}