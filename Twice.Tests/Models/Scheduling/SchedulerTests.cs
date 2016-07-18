using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Twice.Models.Scheduling;

namespace Twice.Tests.Models.Scheduling
{
	[TestClass, ExcludeFromCodeCoverage]
	public class SchedulerTests
	{
		[TestMethod, TestCategory( "Models.Scheduling" )]
		public void FutureJobIsNotExecuted()
		{
			// Arrange
			var scheduler = new Scheduler( "invalid.filename" );
			var job = new SchedulerJob
			{
				TargetTime = DateTime.Now.AddHours( 1 )
			};

			// Act
			bool processed = scheduler.ProcessJob( job );

			// Assert
			Assert.IsFalse( processed );
		}

		[TestMethod, TestCategory( "Models.Scheduling" )]
		public void JobCanBeAdded()
		{
			var fileName = "SchedulerTests.JobCanBeAdded.json";
			try
			{
				// Arrange
				var scheduler = new Scheduler( fileName );

				var job = new SchedulerJob
				{
					JobType = SchedulerJobType.Test,
					AccountId = 123,
					FilesToAttach = new List<string> {"1", "2", "3"},
					IdToDelete = 456,
					InReplyToStatus = 678,
					InReplyToUser = "test",
					TargetTime = new DateTime( 1, 2, 3, 4, 5, 6 ),
					Text = "hello world"
				};

				// Act
				scheduler.AddJob( job );

				// Assert
				string json = File.ReadAllText( fileName );
				var jobList = JsonConvert.DeserializeObject<List<SchedulerJob>>( json );

				Assert.IsNotNull( jobList );
				Assert.AreEqual( 1, jobList.Count );
				Assert.AreEqual( job.AccountId, jobList[0].AccountId );
				Assert.AreEqual( job.InReplyToStatus, jobList[0].InReplyToStatus );
				Assert.AreEqual( job.InReplyToUser, jobList[0].InReplyToUser );
				Assert.AreEqual( job.TargetTime, jobList[0].TargetTime );
				Assert.AreEqual( job.Text, jobList[0].Text );
				Assert.AreEqual( job.JobType, jobList[0].JobType );
				Assert.AreEqual( job.IdToDelete, jobList[0].IdToDelete );
				CollectionAssert.AreEquivalent( job.FilesToAttach, jobList[0].FilesToAttach );
			}
			finally
			{
				File.Delete( fileName );
			}
		}

		[TestMethod, TestCategory( "Models.Scheduling" )]
		public void JobIsExecutedInThread()
		{
			// Arrange
			var waitHandle = new ManualResetEventSlim( false );

			var proc = new Mock<IJobProcessor>();
			proc.Setup( p => p.Process( It.IsAny<SchedulerJob>() ) ).Callback( () => waitHandle.Set() ).Verifiable();

			var scheduler = new Scheduler( "SchedulerTests.JobIsExecutedInThread.json", proc.Object );
			scheduler.Start();
			try
			{
				var job = new SchedulerJob {JobType = SchedulerJobType.Test, TargetTime = DateTime.Now.AddMilliseconds( -1 )};
				scheduler.AddJob( job );

				// Act
				bool set = waitHandle.Wait( 2000 );

				// Assert
				Assert.IsTrue( set );
				proc.Verify( p => p.Process( It.IsAny<SchedulerJob>() ), Times.Once() );
			}
			finally
			{
				scheduler.Stop();
			}
		}

		[TestMethod, TestCategory( "Models.Scheduling" )]
		public void JobListIsLoadedOnConstruction()
		{
			// Arrange
			var fileName = "SchedulerTests.JobListIsLoadedOnConstruction.json";
			var jobList = new List<SchedulerJob>
			{
				new SchedulerJob
				{
					JobType = SchedulerJobType.CreateStatus,
					AccountId = 123,
					InReplyToUser = "test",
					InReplyToStatus = 444,
					TargetTime = new DateTime( 1, 2, 3, 4, 5, 6 ),
					Text = "hello world",
					FilesToAttach = new List<string> {"123", "456", "789"}
				},
				new SchedulerJob {JobType = SchedulerJobType.DeleteStatus, AccountId = 456, IdToDelete = 5678},
				new SchedulerJob {JobType = SchedulerJobType.Test}
			};
			File.WriteAllText( fileName, JsonConvert.SerializeObject( jobList ) );

			// Act
			var scheduler = new Scheduler( fileName );

			// Assert
			Assert.AreEqual( 3, scheduler.JobList.Count() );
			var create = scheduler.JobList.SingleOrDefault( j => j.JobType == SchedulerJobType.CreateStatus );
			Assert.IsNotNull( create );
			Assert.AreEqual( jobList[0].AccountId, create.AccountId );
			Assert.AreEqual( jobList[0].InReplyToStatus, create.InReplyToStatus );
			Assert.AreEqual( jobList[0].InReplyToUser, create.InReplyToUser );
			Assert.AreEqual( jobList[0].TargetTime, create.TargetTime );
			Assert.AreEqual( jobList[0].Text, create.Text );
			CollectionAssert.AreEquivalent( jobList[0].FilesToAttach, create.FilesToAttach );

			var delete = scheduler.JobList.SingleOrDefault( j => j.JobType == SchedulerJobType.DeleteStatus );
			Assert.IsNotNull( delete );
			Assert.AreEqual( jobList[1].AccountId, delete.AccountId );
			Assert.AreEqual( jobList[1].IdToDelete, delete.IdToDelete );

			var test = scheduler.JobList.SingleOrDefault( j => j.JobType == SchedulerJobType.Test );
			Assert.IsNotNull( test );
		}

		[TestMethod, TestCategory( "Models.Scheduling" )]
		public void LoadingBrokenJobListDoesNotCrash()
		{
			// Arrange
			var fileName = "SchedulerTests.LoadingBrokenJobListDoesNotCrash.json";
			File.WriteAllText( fileName, @"This is a broken JSON file" );

			// Act
			var ex = ExceptionAssert.Catch<Exception>( () => new Scheduler( fileName ) );

			// Assert
			Assert.IsNull( ex );
		}

		[TestMethod, TestCategory( "Models.Scheduling" )]
		public void PastJobIsExecuted()
		{
			// Arrange
			var job = new SchedulerJob
			{
				TargetTime = DateTime.Now.AddHours( -1 )
			};

			var proc = new Mock<IJobProcessor>();
			proc.Setup( p => p.Process( job ) ).Verifiable();
			var scheduler = new Scheduler( "invalid.filename", proc.Object );

			// Act
			bool processed = scheduler.ProcessJob( job );

			// Assert
			Assert.IsTrue( processed );
			proc.Verify( p => p.Process( job ), Times.Once() );
		}
	}
}