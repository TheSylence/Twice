using Anotar.NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Twice.Models.Twitter;

namespace Twice.Models.Scheduling.Processors
{
	internal class CreateStatusProcessor : AbstractProcessor
	{
		public CreateStatusProcessor( ITwitterContextList contextList, ITwitterConfiguration twitterConfig )
			: base( contextList )
		{
			TwitterConfig = twitterConfig;
		}

		public override async Task Process( SchedulerJob job )
		{
			List<ulong> mediaIds = new List<ulong>();

			foreach( var file in job.FilesToAttach )
			{
				byte[] mediaData = File.ReadAllBytes( file );
				if( mediaData.Length > TwitterConfig.MaxImageSize )
				{
					LogTo.Warn( "Tried to attach image that was too big" );
					continue;
				}

				var usedAccounts = ContextList.Contexts.Where( c => job.AccountIds.Contains( c.UserId ) ).ToArray();
				var acc = usedAccounts.First();
				var additionalOwners = usedAccounts.Skip( 1 ).Select( a => a.UserId );

				string mediaType = TwitterHelper.GetMimeType( file );
				var media = await acc.Twitter.UploadMediaAsync( mediaData, mediaType, additionalOwners );

				mediaIds.Add( media.MediaID );
			}

			var taskList = new List<Task>();
			foreach( var accountId in job.AccountIds )
			{
				var context = ContextList.Contexts.FirstOrDefault( c => c.UserId == accountId );
				if( context == null )
				{
					LogTo.Warn( $"Account with Id {accountId} was not found" );
					continue;
				}

				var task = context.Twitter.Statuses.TweetAsync( job.Text, mediaIds, job.InReplyToStatus );
				taskList.Add( task );
			}

			await Task.WhenAll( taskList );
		}

		private readonly ITwitterConfiguration TwitterConfig;
	}
}