using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels.Columns;

namespace Twice.Tests.ViewModels.Columns
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ColumnFactoryTests
	{
		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void CorrectColumnIsConstructedForType()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 1 );

			var contextList = new[]
			{
				context.Object
			};

			var contexts = new Mock<ITwitterContextList>();
			contexts.SetupGet( c => c.Contexts ).Returns( contextList );

			var parser = new Mock<IStreamParser>();

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var streamingRepo = new Mock<IStreamingRepository>();
			streamingRepo.Setup( s => s.GetParser( It.Is<ColumnDefinition>( d => d.SourceAccounts.Contains( (ulong)1 ) ) ) )
				.Returns( parser.Object );
			var factory = new ColumnFactory
			{
				Contexts = contexts.Object,
				StreamingRepo = streamingRepo.Object,
				Configuration = config.Object
			};

			var testCases = new Dictionary<ColumnType, Type>
			{
				{ColumnType.Mentions, typeof(MentionsColumn)},
				{ColumnType.User, typeof(UserColumn)},
				{ColumnType.Timeline, typeof(TimelineColumn)}
			};

			// Act & Assert
			foreach( var kvp in testCases )
			{
				var constructed = factory.Construct( new ColumnDefinition( kvp.Key )
				{
					SourceAccounts = new ulong[] {1},
					TargetAccounts = new ulong[] {1}
				} );
				Assert.IsNotNull( constructed );

				var type = constructed.GetType();
				Assert.IsTrue( kvp.Value.IsAssignableFrom( type ) );
			}
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void NoColumnIsConstructedWhenNoContextIsFound()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 2 );

			var contextList = new[]
			{
				context.Object
			};

			var contexts = new Mock<ITwitterContextList>();
			contexts.SetupGet( c => c.Contexts ).Returns( contextList );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var streamingRepo = new Mock<IStreamingRepository>();
			var factory = new ColumnFactory
			{
				Contexts = contexts.Object,
				StreamingRepo = streamingRepo.Object,
				Configuration = config.Object
			};

			// Act
			var constructed = factory.Construct( new ColumnDefinition( ColumnType.User )
			{
				SourceAccounts = new ulong[] {1},
				TargetAccounts = new ulong[] {1}
			} );

			// Assert
			Assert.IsNull( constructed );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void UnknownColumnTypeIsNotConstructed()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 1 );

			var contextList = new[]
			{
				context.Object
			};

			var contexts = new Mock<ITwitterContextList>();
			contexts.SetupGet( c => c.Contexts ).Returns( contextList );

			var parser = new Mock<IStreamParser>();

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var streamingRepo = new Mock<IStreamingRepository>();
			streamingRepo.Setup( s => s.GetParser( It.Is<ColumnDefinition>( d => d.SourceAccounts.Contains( (ulong)1 ) ) ) )
				.Returns( parser.Object );
			var factory = new ColumnFactory
			{
				Contexts = contexts.Object,
				StreamingRepo = streamingRepo.Object,
				Configuration = config.Object
			};

			// Act
			var constructed = factory.Construct( new ColumnDefinition( ColumnType.DebugOrTest )
			{
				SourceAccounts = new ulong[] {1},
				TargetAccounts = new ulong[] {1}
			} );

			// Assert
			Assert.IsNull( constructed );
		}
	}
}