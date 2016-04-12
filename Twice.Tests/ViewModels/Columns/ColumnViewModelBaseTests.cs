using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq.Expressions;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels;
using Twice.ViewModels.Columns;

namespace Twice.Tests.ViewModels.Columns
{
	[TestClass]
	public class ColumnViewModelBaseTests
	{
		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void ChangingWidthRaisesEvent()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			bool raised = false;
			vm.Changed += ( s, e ) => raised = true;

			// Act
			vm.Width *= 1.1;

			// Assert
			Assert.IsTrue( raised );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void ColumnDoesNotStartStreamingItself()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();
			parser.Setup( p => p.StartStreaming() ).Verifiable();

			// Act
			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );

			// Assert
			Assert.IsNotNull( vm );
			parser.Verify( p => p.StartStreaming(), Times.Never() );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			var tester = new PropertyChangedTester( vm, true, new NinjectTypeResolver() );

			// Act
			tester.Test( nameof( ColumnViewModelBase.Muter ), nameof( ColumnViewModelBase.ContextList ), nameof( ColumnViewModelBase.ViewServiceRepository ),
				nameof( ColumnViewModelBase.Cache ), nameof( ColumnViewModelBase.Configuration ) );

			// Assert
			tester.Verify();
		}

		private class TestColumn : ColumnViewModelBase
		{
			public TestColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser )
				: base( context, definition, config, parser )
			{
				StatusFilterExpression = s => true;
				Icon = Icon.User;
			}

			protected override bool IsSuitableForColumn( Status status )
			{
				return true;
			}

			public override Icon Icon { get; }
			protected override Expression<Func<Status, bool>> StatusFilterExpression { get; }
		}
	}
}