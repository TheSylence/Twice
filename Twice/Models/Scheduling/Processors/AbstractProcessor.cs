using System.Threading.Tasks;
using Twice.Models.Twitter;

namespace Twice.Models.Scheduling.Processors
{
	internal abstract class AbstractProcessor : IJobProcessor
	{
		protected AbstractProcessor( ITwitterContextList contextList )
		{
			ContextList = contextList;
		}

		public abstract Task Process( SchedulerJob job );

		protected ITwitterContextList ContextList { get; }
	}
}