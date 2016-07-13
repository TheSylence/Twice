using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
			var scheduler = new Scheduler();
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
		public void PastJobIsExecuted()
		{
			// Arrange
			var job = new SchedulerJob
			{
				TargetTime = DateTime.Now.AddHours( -1 )
			};

			var proc = new Mock<IJobProcessor>();
			proc.Setup( p => p.Process( job ) ).Verifiable();
			var scheduler = new Scheduler( proc.Object );

			// Act
			bool processed = scheduler.ProcessJob( job );

			// Assert
			Assert.IsTrue( processed );
			proc.Verify( p => p.Process( job ), Times.Once() );
		}
	}
}