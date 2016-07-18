namespace Twice.Models.Scheduling
{
	internal interface IScheduler
	{
		void Start();

		void Stop();
		void AddJob( SchedulerJob job );
	}
}