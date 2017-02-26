using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Scheduling;
using Twice.Utilities;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass]
	public class ScheduleInformationViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void DeletionDateMustBeInFutureWhenEnabled()
		{
			// Arrange
			var vm = new ScheduleInformationViewModel( null, new DateProvider() );

			var pastDate = DateTime.Now.AddDays( -1 );
			var futureDate = DateTime.Now.AddDays( 1 );

			// Act
			vm.IsDeletionScheduled = false;
			vm.DeletionDate = pastDate;
			bool disabledDateInPast = vm.GetErrors( nameof( ScheduleInformationViewModel.DeletionDate ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = false;
			vm.DeletionDate = futureDate;
			bool disabledDateInFuture = vm.GetErrors( nameof( ScheduleInformationViewModel.DeletionDate ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = true;
			vm.DeletionDate = pastDate;
			bool enabledDateInPast = vm.GetErrors( nameof( ScheduleInformationViewModel.DeletionDate ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = true;
			vm.DeletionDate = futureDate;
			bool enabledDateInFuture = vm.GetErrors( nameof( ScheduleInformationViewModel.DeletionDate ) ).Cast<object>().Any();

			// Assert
			Assert.IsFalse( disabledDateInPast );
			Assert.IsFalse( disabledDateInFuture );
			Assert.IsTrue( enabledDateInPast );
			Assert.IsFalse( enabledDateInFuture );
		}
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void DeletionTimeMustBeInFutureWhenEnabled()
		{
			// Arrange
			var vm = new ScheduleInformationViewModel( null, new DateProvider() );

			var pastDate = DateTime.Now.AddHours( -1 );
			var futureDate = DateTime.Now.AddHours( 1 );

			// Act
			vm.IsDeletionScheduled = false;
			vm.DeletionTime = pastDate;
			vm.DeletionDate = pastDate.Date;
			bool disabledDateInPast = vm.GetErrors( nameof( ScheduleInformationViewModel.DeletionTime ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = false;
			vm.DeletionTime = futureDate;
			vm.DeletionDate = futureDate.Date;
			bool disabledDateInFuture = vm.GetErrors( nameof( ScheduleInformationViewModel.DeletionTime ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = true;
			vm.DeletionTime = pastDate;
			vm.DeletionDate = pastDate.Date;
			bool enabledDateInPast = vm.GetErrors( nameof( ScheduleInformationViewModel.DeletionTime ) ).Cast<object>().Any();

			vm.IsDeletionScheduled = true;
			vm.DeletionTime = futureDate;
			vm.DeletionDate = futureDate.Date;
			bool enabledDateInFuture = vm.GetErrors( nameof( ScheduleInformationViewModel.DeletionTime ) ).Cast<object>().Any();

			// Assert
			Assert.IsFalse( disabledDateInPast );
			Assert.IsFalse( disabledDateInFuture );
			Assert.IsTrue( enabledDateInPast );
			Assert.IsFalse( enabledDateInFuture );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ScheduleDateMustBeInFutureWhenEnabled()
		{
			// Arrange
			var vm = new ScheduleInformationViewModel( null, new DateProvider() );

			var pastDate = DateTime.Now.AddDays( -1 );
			var futureDate = DateTime.Now.AddDays( 1 );

			// Act
			vm.IsTweetScheduled = false;
			vm.ScheduleDate = pastDate;
			bool disabledDateInPast = vm.GetErrors( nameof( ScheduleInformationViewModel.ScheduleDate ) ).Cast<object>().Any();

			vm.IsTweetScheduled = false;
			vm.ScheduleDate = futureDate;
			bool disabledDateInFuture = vm.GetErrors( nameof( ScheduleInformationViewModel.ScheduleDate ) ).Cast<object>().Any();

			vm.IsTweetScheduled = true;
			vm.ScheduleDate = pastDate;
			bool enabledDateInPast = vm.GetErrors( nameof( ScheduleInformationViewModel.ScheduleDate ) ).Cast<object>().Any();

			vm.IsTweetScheduled = true;
			vm.ScheduleDate = futureDate;
			bool enabledDateInFuture = vm.GetErrors( nameof( ScheduleInformationViewModel.ScheduleDate ) ).Cast<object>().Any();

			// Assert
			Assert.IsFalse( disabledDateInPast );
			Assert.IsFalse( disabledDateInFuture );
			Assert.IsTrue( enabledDateInPast );
			Assert.IsFalse( enabledDateInFuture );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ScheduledTimeMustBeInFutureWhenEnabled()
		{
			// Arrange
			var vm = new ScheduleInformationViewModel( null, new DateProvider() );

			var pastDate = DateTime.Now.AddHours( -1 );
			var futureDate = DateTime.Now.AddHours( 1 );

			// Act
			vm.IsTweetScheduled = false;
			vm.ScheduleTime = pastDate;
			vm.ScheduleDate = pastDate.Date;
			bool disabledDateInPast = vm.GetErrors( nameof( ScheduleInformationViewModel.ScheduleTime ) ).Cast<object>().Any();

			vm.IsTweetScheduled = false;
			vm.ScheduleTime = futureDate;
			vm.ScheduleDate = futureDate.Date;
			bool disabledDateInFuture = vm.GetErrors( nameof( ScheduleInformationViewModel.ScheduleTime ) ).Cast<object>().Any();

			vm.IsTweetScheduled = true;
			vm.ScheduleTime = pastDate;
			vm.ScheduleDate = pastDate.Date;
			bool enabledDateInPast = vm.GetErrors( nameof( ScheduleInformationViewModel.ScheduleTime ) ).Cast<object>().Any();

			vm.IsTweetScheduled = true;
			vm.ScheduleTime = futureDate;
			vm.ScheduleDate = futureDate.Date;
			bool enabledDateInFuture = vm.GetErrors( nameof( ScheduleInformationViewModel.ScheduleTime ) ).Cast<object>().Any();

			// Assert
			Assert.IsFalse( disabledDateInPast );
			Assert.IsFalse( disabledDateInFuture );
			Assert.IsTrue( enabledDateInPast );
			Assert.IsFalse( enabledDateInFuture );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void ScheduleJobIsCreatedIfNeeded()
		{
			// Arrange
			Expression<Func<SchedulerJob, bool>> jobVerifier =
				job => job.JobType == SchedulerJobType.CreateStatus
				       && job.Text == "Hello World";

			var scheduler = new Mock<IScheduler>();
			scheduler.Setup( s => s.AddJob( It.Is( jobVerifier ) ) ).Verifiable();

			var vm = new ScheduleInformationViewModel( scheduler.Object, new DateProvider() )
			{
				IsTweetScheduled = true,
				ScheduleDate = DateTime.Now.AddDays( 10 ),
				ScheduleTime = DateTime.Now.AddMinutes( 1 )
			};

			// Act
			vm.ScheduleTweet( "Hello World", null, Enumerable.Empty<ulong>(), Enumerable.Empty<string>() );

			// Assert
			scheduler.Verify( s => s.AddJob( It.Is( jobVerifier ) ), Times.Once() );
		}
	}
}