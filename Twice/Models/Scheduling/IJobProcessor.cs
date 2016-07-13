namespace Twice.Models.Scheduling
{
	internal interface IJobProcessor
	{
		void Process( SchedulerJob job );
	}
}