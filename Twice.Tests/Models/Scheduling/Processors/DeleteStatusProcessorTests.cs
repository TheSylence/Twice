using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Scheduling;
using Twice.Models.Scheduling.Processors;
using Twice.Models.Twitter;

namespace Twice.Tests.Models.Scheduling.Processors
{
	[TestClass, ExcludeFromCodeCoverage]
	public class DeleteStatusProcessorTests
	{
		[TestMethod, TestCategory( "Models.Scheduling.Processors" )]
		public async Task StatusFromUnknownAccountIsNotDeleted()
		{
			// Arrange
			var job = new SchedulerJob
			{
				JobType = SchedulerJobType.DeleteStatus,
				IdsToDelete = new List<ulong> {1001, 2001},
				AccountIds = new List<ulong> {100, 200}
			};

			var ctx1 = new Mock<IContextEntry>();
			ctx1.Setup( c => c.UserId ).Returns( 1 );
			ctx1.Setup( c => c.Twitter.Statuses.DeleteTweetAsync( It.IsAny<ulong>() ) ).Returns( Task.FromResult( DummyGenerator.CreateDummyStatus() ) ).Verifiable();

			var ctx2 = new Mock<IContextEntry>();
			ctx2.Setup( c => c.UserId ).Returns( 2 );
			ctx2.Setup( c => c.Twitter.Statuses.DeleteTweetAsync( It.IsAny<ulong>() ) ).Returns( Task.FromResult( DummyGenerator.CreateDummyStatus() ) ).Verifiable();

			var contextList = new Mock<ITwitterContextList>();
			contextList.Setup( c => c.Contexts ).Returns( new[] {ctx1.Object, ctx2.Object} );

			var proc = new DeleteStatusProcessor( contextList.Object );

			// Act
			await proc.Process( job );

			// Assert
			ctx1.Verify( c => c.Twitter.Statuses.DeleteTweetAsync( It.IsAny<ulong>() ), Times.Never() );
			ctx2.Verify( c => c.Twitter.Statuses.DeleteTweetAsync( It.IsAny<ulong>() ), Times.Never() );
		}

		[TestMethod, TestCategory( "Models.Scheduling.Processors" )]
		public async Task StatusIsDeleteFromCorrectAccount()
		{
			// Arrange
			var job = new SchedulerJob
			{
				JobType = SchedulerJobType.DeleteStatus,
				IdsToDelete = new List<ulong> {1001, 2001},
				AccountIds = new List<ulong> {1, 2}
			};

			var ctx1 = new Mock<IContextEntry>();
			ctx1.Setup( c => c.UserId ).Returns( 1 );
			ctx1.Setup( c => c.Twitter.Statuses.DeleteTweetAsync( 1001 ) ).Returns( Task.FromResult( DummyGenerator.CreateDummyStatus() ) ).Verifiable();

			var ctx2 = new Mock<IContextEntry>();
			ctx2.Setup( c => c.UserId ).Returns( 2 );
			ctx2.Setup( c => c.Twitter.Statuses.DeleteTweetAsync( 2001 ) ).Returns( Task.FromResult( DummyGenerator.CreateDummyStatus() ) ).Verifiable();

			var contextList = new Mock<ITwitterContextList>();
			contextList.Setup( c => c.Contexts ).Returns( new[] {ctx1.Object, ctx2.Object} );

			var proc = new DeleteStatusProcessor( contextList.Object );

			// Act
			await proc.Process( job );

			// Assert
			ctx1.Verify( c => c.Twitter.Statuses.DeleteTweetAsync( 1001 ), Times.Once() );
			ctx2.Verify( c => c.Twitter.Statuses.DeleteTweetAsync( 2001 ), Times.Once() );
		}
	}
}