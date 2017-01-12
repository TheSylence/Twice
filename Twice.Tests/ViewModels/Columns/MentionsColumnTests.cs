using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels.Columns;

namespace Twice.Tests.ViewModels.Columns
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MentionsColumnTests
	{
		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void MessageIsAlwaysRejected()
		{
			// Arrange
			var vm = new TestColumn();

			// Act
			bool suitable = vm.Suitable( new DirectMessage() );

			// Assert
			Assert.IsFalse( suitable );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void StatusWithMentionsForOtherUsersIsRejected()
		{
			// Arrange
			var vm = new TestColumn();
			var status = DummyGenerator.CreateDummyStatus();
			status.Entities.UserMentionEntities.Add( new UserMentionEntity
			{
				Id = 123
			} );

			// Act
			bool suitable = vm.Suitable( status );

			// Assert
			Assert.IsFalse( suitable );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void StatusWithoutMentionIsRejected()
		{
			// Arrange
			var vm = new TestColumn();
			var status = DummyGenerator.CreateDummyStatus();

			// Act
			bool suitable = vm.Suitable( status );

			// Assert
			Assert.IsFalse( suitable );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void StatusWithUserMentionIsAccepted()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 123 );

			var vm = new TestColumn( context.Object );
			var status = DummyGenerator.CreateDummyStatus();
			status.Entities.UserMentionEntities.Add( new UserMentionEntity { Id = 123 } );

			// Act
			bool suitable = vm.Suitable( status );

			// Assert
			Assert.IsTrue( suitable );
		}

		private class TestColumn : MentionsColumn
		{
			public TestColumn( IContextEntry context = null, IConfig config = null, IStreamParser parser = null )
				: base(
					context ?? DefaultContext(), new ColumnDefinition( ColumnType.Mentions ), config ?? DefaultConfig(),
					parser ?? DefaultParser() )
			{
			}

			public bool Suitable( DirectMessage message )
			{
				return IsSuitableForColumn( message );
			}

			public bool Suitable( Status status )
			{
				return IsSuitableForColumn( status );
			}

			private static IConfig DefaultConfig()
			{
				var cfg = new Mock<IConfig>();
				cfg.SetupGet( c => c.General ).Returns( new GeneralConfig() );

				return cfg.Object;
			}

			private static IContextEntry DefaultContext()
			{
				var context = new Mock<IContextEntry>();
				context.SetupGet( c => c.AccountName ).Returns( string.Empty );

				return context.Object;
			}

			private static IStreamParser DefaultParser()
			{
				var parser = new Mock<IStreamParser>();

				return parser.Object;
			}
		}
	}
}