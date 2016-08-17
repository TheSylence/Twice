using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Twice.Models.Scheduling;
using Twice.Models.Twitter;

namespace Twice.Tests.Models.Scheduling
{
	[TestClass, ExcludeFromCodeCoverage]
	public class SchedulerTests
	{
		[TestMethod, TestCategory( "Models.Scheduling" )]
		public void FutureJobIsNotExecuted()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var config = new Mock<ITwitterConfiguration>();

			var scheduler = new Scheduler( "invalid.filename", contextList.Object, config.Object );
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
				var contextList = new Mock<ITwitterContextList>();
				var config = new Mock<ITwitterConfiguration>();

				var scheduler = new Scheduler( fileName, contextList.Object, config.Object );

				var job = new SchedulerJob
				{
					JobType = SchedulerJobType.Test,
					AccountIds = new List<ulong> {123},
					FilesToAttach = new List<string> {"1", "2", "3"},
					IdsToDelete = new List<ulong> {456},
					InReplyToStatus = 678,
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
				CollectionAssert.AreEquivalent( job.AccountIds, jobList[0].AccountIds );
				Assert.AreEqual( job.InReplyToStatus, jobList[0].InReplyToStatus );
				Assert.AreEqual( job.TargetTime, jobList[0].TargetTime );
				Assert.AreEqual( job.Text, jobList[0].Text );
				Assert.AreEqual( job.JobType, jobList[0].JobType );
				CollectionAssert.AreEquivalent( job.IdsToDelete, jobList[0].IdsToDelete );
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
			var contextList = new Mock<ITwitterContextList>();
			var config = new Mock<ITwitterConfiguration>();

			var waitHandle = new ManualResetEventSlim( false );

			var proc = new Mock<IJobProcessor>();
			proc.Setup( p => p.Process( It.IsAny<SchedulerJob>() ) ).Returns( Task.CompletedTask ).Callback(
				() => waitHandle.Set() ).Verifiable();

			var scheduler = new Scheduler( "SchedulerTests.JobIsExecutedInThread.json", contextList.Object, config.Object,
				proc.Object );
			scheduler.Start();
			try
			{
				var job = new SchedulerJob
				{
					JobType = SchedulerJobType.Test,
					TargetTime = DateTime.Now.AddMilliseconds( -1 ),
					AccountIds = new List<ulong> {1},
					Text = "test"
				};
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
			var contextList = new Mock<ITwitterContextList>();
			var config = new Mock<ITwitterConfiguration>();

			var fileName = "SchedulerTests.JobListIsLoadedOnConstruction.json";
			var jobList = new List<SchedulerJob>
			{
				new SchedulerJob
				{
					JobType = SchedulerJobType.CreateStatus,
					AccountIds = new List<ulong> {123},
					InReplyToStatus = 444,
					TargetTime = new DateTime( 1, 2, 3, 4, 5, 6 ),
					Text = "hello world",
					FilesToAttach = new List<string> {"123", "456", "789"}
				},
				new SchedulerJob
				{
					JobType = SchedulerJobType.DeleteStatus,
					AccountIds = new List<ulong> {456},
					IdsToDelete = new List<ulong> {5678}
				},
				new SchedulerJob {JobType = SchedulerJobType.Test}
			};
			File.WriteAllText( fileName, JsonConvert.SerializeObject( jobList ) );

			// Act
			var scheduler = new Scheduler( fileName, contextList.Object, config.Object );

			// Assert
			Assert.AreEqual( 3, scheduler.JobList.Count() );
			var create = scheduler.JobList.SingleOrDefault( j => j.JobType == SchedulerJobType.CreateStatus );
			Assert.IsNotNull( create );
			CollectionAssert.AreEquivalent( jobList[0].AccountIds, create.AccountIds );
			Assert.AreEqual( jobList[0].InReplyToStatus, create.InReplyToStatus );
			Assert.AreEqual( jobList[0].TargetTime, create.TargetTime );
			Assert.AreEqual( jobList[0].Text, create.Text );
			CollectionAssert.AreEquivalent( jobList[0].FilesToAttach, create.FilesToAttach );

			var delete = scheduler.JobList.SingleOrDefault( j => j.JobType == SchedulerJobType.DeleteStatus );
			Assert.IsNotNull( delete );
			CollectionAssert.AreEquivalent( jobList[1].AccountIds, delete.AccountIds );
			CollectionAssert.AreEquivalent( jobList[1].IdsToDelete, delete.IdsToDelete );

			var test = scheduler.JobList.SingleOrDefault( j => j.JobType == SchedulerJobType.Test );
			Assert.IsNotNull( test );
		}

		[TestMethod, TestCategory( "Models.Scheduling" )]
		public void LoadingBrokenJobListDoesNotCrash()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var config = new Mock<ITwitterConfiguration>();

			var fileName = "SchedulerTests.LoadingBrokenJobListDoesNotCrash.json";
			File.WriteAllText( fileName, @"This is a broken JSON file" );

			// Act
			var ex = ExceptionAssert.Catch<Exception>( () => new Scheduler( fileName, contextList.Object, config.Object ) );

			// Assert
			Assert.IsNull( ex );
		}

		[TestMethod, TestCategory( "Models.Scheduling" )]
		public void PastJobIsExecuted()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var config = new Mock<ITwitterConfiguration>();

			var job = new SchedulerJob
			{
				TargetTime = DateTime.Now.AddHours( -1 )
			};

			var proc = new Mock<IJobProcessor>();
			proc.Setup( p => p.Process( job ) ).Verifiable();
			var scheduler = new Scheduler( "invalid.filename", contextList.Object, config.Object, proc.Object );

			// Act
			bool processed = scheduler.ProcessJob( job );

			// Assert
			Assert.IsTrue( processed );
			proc.Verify( p => p.Process( job ), Times.Once() );
		}
	}
}