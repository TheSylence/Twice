using System;
using System.Collections.Generic;

namespace Twice.Models.Scheduling
{
	internal interface IScheduler
	{
		void AddJob( SchedulerJob job );

		void DeleteJob( SchedulerJob job );

		void Start();

		void Stop();

		IEnumerable<SchedulerJob> JobList { get; }
		event EventHandler JobListUpdated;
	}
}