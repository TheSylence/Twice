using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using Twice.Models.Twitter;

namespace Twice.Models.Scheduling.Processors
{
	internal class DeleteStatusProcessor : AbstractProcessor
	{
		public DeleteStatusProcessor( ITwitterContextList contextList )
			: base( contextList )
		{
		}

		public override async Task Process( SchedulerJob job )
		{
			for( int i = 0; i < job.IdsToDelete.Count; ++i )
			{
				var statusId = job.IdsToDelete[i];
				var accountId = job.AccountIds[i];

				var context = ContextList.Contexts.FirstOrDefault( c => c.UserId == accountId );
				if( context == null )
				{
					LogTo.Warn( $"Account with Id ({accountId}) was not found" );
					continue;
				}

				await context.Twitter.Statuses.DeleteTweetAsync( statusId );
			}
		}
	}
}