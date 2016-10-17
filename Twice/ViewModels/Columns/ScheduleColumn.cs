using GalaSoft.MvvmLight.Messaging;
using LinqToTwitter;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Scheduling;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.Resources;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	internal class ScheduleColumn : ColumnViewModelBase

	{
		public ScheduleColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser,
			IScheduler scheduler, IMessenger messenger = null )
			: base( context, definition, config, parser, messenger )
		{
			Scheduler = scheduler;
			Title = Strings.Schedule;

			Scheduler.JobListUpdated += Scheduler_JobListUpdated;
		}

		protected override bool IsSuitableForColumn( Status status )
		{
			return false;
		}

		protected override bool IsSuitableForColumn( DirectMessage message )
		{
			return false;
		}

		protected override async Task OnLoad()
		{
			await SetJobs();
		}

		private async Task<ScheduleItem> CreateViewModel( SchedulerJob job )
		{
			var userId = job.AccountIds.First();
			var user = await Cache.GetUser( userId );
			if( user == null )
			{
				user = await Context.Twitter.Users.ShowUser( userId, true );
				await Cache.AddUsers( new[]
				{
					new UserCacheEntry( user )
				} );
			}

			var userVm = new UserViewModel( user );
			var item = new ScheduleItem( job, userVm, Scheduler, Configuration, ViewServiceRepository );
			return item;
		}

		private async void Scheduler_JobListUpdated( object sender, EventArgs e )
		{
			await SetJobs();
		}

		private async Task SetJobs()
		{
			await Dispatcher.RunAsync( () => Items.Clear() );

			foreach( var job in Scheduler.JobList.ToArray() )
			{
				await AddItem( await CreateViewModel( job ) );
			}
		}

		public override Icon Icon => Icon.Schedule;
		protected override Expression<Func<Status, bool>> StatusFilterExpression => s => false;
		private readonly IScheduler Scheduler;
	}
}