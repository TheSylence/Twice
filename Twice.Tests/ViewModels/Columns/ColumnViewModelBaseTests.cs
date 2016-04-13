using System;
using System.Linq.Expressions;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;
using Twice.ViewModels;
using Twice.ViewModels.Columns;
using Twice.ViewModels.Twitter;

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
		public void ClearCommandClearsAllStatuses()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			vm.Statuses.Add( new StatusViewModel( CreateDummyStatus(), context.Object ) );
			vm.Statuses.Add( new StatusViewModel( CreateDummyStatus(), context.Object ) );
			vm.Statuses.Add( new StatusViewModel( CreateDummyStatus(), context.Object ) );

			// Act
			int countBefore = vm.Statuses.Count;
			vm.ClearCommand.Execute( null );
			int countAfter = vm.Statuses.Count;

			// Assert
			Assert.AreNotEqual( 0, countBefore );
			Assert.AreEqual( 0, countAfter );
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
		public void DeleteCommandRaisesEvent()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			bool raised = false;
			vm.Deleted += ( s, e ) => raised = true;

			// Act
			vm.DeleteCommand.Execute( null );

			// Assert
			Assert.IsTrue( raised );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void NewStatusEventWithoutSubscribersDoesNotCrash()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );

			// Act
			vm.RaiseStatusWrapper( new StatusViewModel( CreateDummyStatus(), context.Object ) );

			// Assert
			Assert.IsTrue( true ); // HACK: This is ugly...
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void NewStatusIsOnlyRaisedWhenNotLoading()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			bool raised = false;
			vm.NewStatus += ( s, e ) => raised = true;

			var status = new StatusViewModel( CreateDummyStatus(), context.Object );

			// Act
			vm.SetLoading( true );
			vm.RaiseStatusWrapper( status );
			bool whileLoading = raised;

			vm.SetLoading( false );
			vm.RaiseStatusWrapper( status );
			bool afterLoading = raised;

			// Assert
			Assert.IsFalse( whileLoading );
			Assert.IsTrue( afterLoading );
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

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void SavingConfigurationRaisesChangedEvent()
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
			vm.ColumnConfiguration.ToastsEnabled = !vm.ColumnConfiguration.ToastsEnabled;
			vm.ColumnConfiguration.SaveCommand.Execute( null );

			// Assert
			Assert.IsTrue( raised );
		}

		private static Status CreateDummyStatus( User user = null )
		{
			user = user ?? CreateDummyUser();

			return new Status
			{
				User = user
			};
		}

		private static User CreateDummyUser()
		{
			return new User
			{
				ProfileImageUrl = "http://example.com/image_normal.png",
				ProfileImageUrlHttps = "https://example.com/image_normal.png"
			};
		}

		private class TestColumn : ColumnViewModelBase
		{
			public TestColumn( IContextEntry context, ColumnDefinition definition, IConfig config, IStreamParser parser )
				: base( context, definition, config, parser )
			{
				StatusFilterExpression = s => true;
				Icon = Icon.User;
			}

			public void RaiseStatusWrapper( StatusViewModel status )
			{
				RaiseNewStatus( status );
			}

			public void SetLoading( bool isLoading )
			{
				IsLoading = isLoading;
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