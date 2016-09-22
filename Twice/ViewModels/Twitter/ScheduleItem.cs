using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Models.Scheduling;
using Twice.Models.Twitter.Entities;
using Twice.Resources;
using Twice.Views.Services;

namespace Twice.ViewModels.Twitter
{
	internal class ScheduleItem : ColumnItem
	{
		public ScheduleItem( SchedulerJob job, UserViewModel user, IScheduler scheduler, IViewServiceRepository viewServices )
		{
			Job = job;
			User = user;
			Scheduler = scheduler;
			ViewServices = viewServices;

			Entities = new Entities
			{
				HashTagEntities = EntityParser.ExtractHashtags( job.Text ),
				MediaEntities = new List<MediaEntity>(),
				SymbolEntities = new List<SymbolEntity>(),
				UrlEntities = new List<UrlEntity>(),
				UserMentionEntities = EntityParser.ExtractMentions( job.Text )
			};
		}

		private async void ExecuteDeleteScheduleCommand()
		{
			var csa = new ConfirmServiceArgs( Strings.ConfirmDeleteSchedule );
			if( !await ViewServices.Confirm( csa ) )
			{
				return;
			}

			Scheduler.DeleteJob( Job );
		}

		public override DateTime CreatedAt => Job.TargetTime;

		public ICommand DeleteScheduleCommand
			=> _DeleteScheduleCommand ?? ( _DeleteScheduleCommand = new RelayCommand( ExecuteDeleteScheduleCommand ) );

		public override Entities Entities { get; }
		public override ulong Id => Job.JobId;
		public override ulong OrderId => Id;
		public DateTime TargetDate => Job.TargetTime;
		public override string Text => Job.Text;
		public SchedulerJobType Type => Job.JobType;
		private readonly SchedulerJob Job;
		private readonly IScheduler Scheduler;
		private readonly IViewServiceRepository ViewServices;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _DeleteScheduleCommand;
	}
}