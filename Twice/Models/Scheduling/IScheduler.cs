using System;
using System.Collections.Generic;

namespace Twice.Models.Scheduling
{
	internal interface IScheduler
	{
		event EventHandler JobListUpdated;

		void AddJob( SchedulerJob job );

		void Start();

		void Stop();

		IEnumerable<SchedulerJob> JobList { get; }
	}
}