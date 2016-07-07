using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels.Columns;

namespace Twice.Tests.ViewModels.Columns
{
	[TestClass, ExcludeFromCodeCoverage]
	public class UserColumnTests
	{
		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void FilterIsWorkingAsExpected()
		{
			// Arrange
			var column = new TestUserColumn();
			
			var s1 = new Status {UserID = 123};
			var s2 = new Status {UserID = 222};

			// Act
			bool b1 = column.FilterExpression( s1 );
			bool b2 = column.FilterExpression( s2 );

			// Assert
			Assert.IsTrue( b1 );
			Assert.IsFalse( b2 );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void MessageIsAlwaysRejected()
		{
			// Arrange
			var column = new TestUserColumn();

			// Act
			bool suitable = column.Suitable( new DirectMessage() );

			// Assert
			Assert.IsFalse( suitable );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void StatusOfDifferentUserIsRejected()
		{
			// Arrange
			var column = new TestUserColumn();

			var otherUser = DummyGenerator.CreateDummyUser( 789 );
			var status = DummyGenerator.CreateDummyStatus( otherUser );

			// Act
			bool suitable = column.Suitable( status );

			// Assert
			Assert.IsFalse( suitable );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void StatusOfUserIsSuitable()
		{
			// Arrange
			var column = new TestUserColumn();

			var thisUser = DummyGenerator.CreateDummyUser( 123 );
			var status = DummyGenerator.CreateDummyStatus( thisUser );

			// Act
			bool suitable = column.Suitable( status );

			// Assert
			Assert.IsTrue( suitable );
		}

		private class TestUserColumn : UserColumn
		{
			public TestUserColumn( IContextEntry context = null, ColumnDefinition definition = null, IConfig config = null, IStreamParser parser = null )
				: base( context ?? DefaultContext(), definition ?? DefaultDefinition(), config ?? DefaultConfig(), parser ?? DefaultParser() )
			{
			}

			public bool FilterExpression( Status status )
			{
				var func = StatusFilterExpression.Compile();
				return func( status );
			}

			public bool Suitable( Status status )
			{
				return IsSuitableForColumn( status );
			}

			public bool Suitable( DirectMessage msg )
			{
				return IsSuitableForColumn( msg );
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

				return context.Object;
			}

			private static ColumnDefinition DefaultDefinition()
			{
				return new ColumnDefinition( ColumnType.User )
				{
					TargetAccounts = new ulong[] {123}
				};
			}

			private static IStreamParser DefaultParser()
			{
				var parser = new Mock<IStreamParser>();

				return parser.Object;
			}
		}
	}
}