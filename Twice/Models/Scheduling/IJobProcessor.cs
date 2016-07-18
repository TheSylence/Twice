using System.Threading.Tasks;

namespace Twice.Models.Scheduling
{
	internal interface IJobProcessor
	{
		Task Process( SchedulerJob job );
	}
}