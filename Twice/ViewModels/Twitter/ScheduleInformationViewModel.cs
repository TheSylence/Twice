using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Twice.Models.Scheduling;
using Twice.Resources;
using Twice.ViewModels.Validation;

namespace Twice.ViewModels.Twitter
{
	class ScheduleInformationViewModel : ValidationViewModel, IScheduleInformationViewModel
	{
		public ScheduleInformationViewModel( IScheduler scheduler )
		{
			Scheduler = scheduler;

			Validate( () => ScheduleDate )
				.If( () => IsTweetScheduled )
				.Check( dt => dt.Date >= DateTime.Now.Date )
				.Message( Strings.DateMustBeInTheFuture );

			Validate( () => ScheduleTime )
				.If( () => IsTweetScheduled )
				.Check( dt => dt.TimeOfDay >= DateTime.Now.TimeOfDay || ScheduleDate.Date > DateTime.Now.Date )
				.Message( Strings.DateMustBeInTheFuture );

			Validate( () => DeletionDate )
				.If( () => IsDeletionScheduled )
				.Check( dt => dt.Date >= DateTime.Now.Date )
				.Message( Strings.DateMustBeInTheFuture );

			Validate( () => DeletionTime )
				.If( () => IsDeletionScheduled )
				.Check( dt => dt.TimeOfDay >= DateTime.Now.TimeOfDay || DeletionDate.Date > DateTime.Now.Date )
				.Message( Strings.DateMustBeInTheFuture );

			Validate( () => DeletionDate )
				.If( () => IsDeletionScheduled && IsTweetScheduled )
				.Check( dt => dt.Date >= ScheduleDate.Date )
				.Message( Strings.DateMustBeInTheFuture );

			Validate( () => DeletionTime )
				.If( () => IsDeletionScheduled && IsTweetScheduled )
				.Check( dt => dt.TimeOfDay >= ScheduleDate.TimeOfDay || DeletionDate.Date > ScheduleDate.Date )
				.Message( Strings.DateMustBeInTheFuture );
		}

		public void ResetSchedule()
		{
			ScheduleDate = DateTime.Now;
			ScheduleTime = DateTime.Now;
			DeletionDate = DateTime.Now;
			DeletionTime = DateTime.Now;
			IsTweetScheduled = false;
			IsDeletionScheduled = false;
		}

		public void ScheduleDeletion( List<Tuple<ulong, ulong>> tweetIds, string text )
		{
			var job = new SchedulerJob
			{
				JobType = SchedulerJobType.DeleteStatus,
				IdsToDelete = tweetIds.Select( t => t.Item1 ).ToList(),
				AccountIds = tweetIds.Select( t => t.Item2 ).ToList(),
				TargetTime = DeletionDate + DeletionTime.TimeOfDay,
				Text = text
			};

			Scheduler.AddJob( job );
		}

		public void ScheduleTweet( string text, ulong? inReplyTo, IEnumerable<ulong> accountIds, IEnumerable<string> mediaFileNames )
		{
			var job = new SchedulerJob
			{
				JobType = SchedulerJobType.CreateStatus,
				Text = text,
				AccountIds = accountIds.ToList(),
				TargetTime = ScheduleDate + ScheduleTime.TimeOfDay,
				InReplyToStatus = inReplyTo ?? 0,
				FilesToAttach = mediaFileNames.ToList()
			};

			Scheduler.AddJob( job );
		}

		public DateTime DeletionDate
		{
			[DebuggerStepThrough] get { return _DeletionDate; }
			set
			{
				if( _DeletionDate == value )
				{
					return;
				}

				_DeletionDate = value;
				RaisePropertyChanged();
			}
		}

		public DateTime DeletionTime
		{
			[DebuggerStepThrough] get { return _DeletionTime; }
			set
			{
				if( _DeletionTime == value )
				{
					return;
				}

				_DeletionTime = value;
				RaisePropertyChanged();
			}
		}

		public bool IsDeletionScheduled
		{
			[DebuggerStepThrough] get { return _IsDeletionScheduled; }
			set
			{
				if( _IsDeletionScheduled == value )
				{
					return;
				}

				_IsDeletionScheduled = value;
				RaisePropertyChanged();

				if( value )
				{
					IsTweetScheduled = false;
				}
			}
		}

		public bool IsTweetScheduled
		{
			[DebuggerStepThrough] get { return _IsTweetScheduled; }
			set
			{
				if( _IsTweetScheduled == value )
				{
					return;
				}

				_IsTweetScheduled = value;
				RaisePropertyChanged();

				if( value )
				{
					IsDeletionScheduled = false;
				}
			}
		}

		public DateTime ScheduleDate
		{
			[DebuggerStepThrough] get { return _ScheduleDate; }
			set
			{
				if( _ScheduleDate == value )
				{
					return;
				}

				_ScheduleDate = value;
				RaisePropertyChanged();
			}
		}

		public IScheduler Scheduler { get; }

		public DateTime ScheduleTime
		{
			[DebuggerStepThrough] get { return _ScheduleTime; }
			set
			{
				if( _ScheduleTime == value )
				{
					return;
				}

				_ScheduleTime = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private DateTime _DeletionDate;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private DateTime _DeletionTime;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsDeletionScheduled;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsTweetScheduled;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private DateTime _ScheduleDate;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private DateTime _ScheduleTime;
	}
}