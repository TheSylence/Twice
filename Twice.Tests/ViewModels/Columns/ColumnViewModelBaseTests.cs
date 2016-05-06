﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Repositories;
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
			vm.Statuses.Add( new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null ) );
			vm.Statuses.Add( new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null ) );
			vm.Statuses.Add( new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null ) );

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
		public async Task InitialLoadingAddsStatuses()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var statuses = new List<Status>
			{
				DummyGenerator.CreateDummyStatus(),
				DummyGenerator.CreateDummyStatus(),
				DummyGenerator.CreateDummyStatus()
			};

			var twitterContext = new Mock<ITwitterContext>();

			var statusRepo = new Mock<ITwitterStatusRepository>();
			statusRepo.Setup( s => s.Filter( It.IsAny<Expression<Func<Status, bool>>>() ) ).Returns( Task.FromResult( statuses ) );
			twitterContext.SetupGet( c => c.Statuses ).Returns( statusRepo.Object );
			context.Setup( c => c.Twitter ).Returns( twitterContext.Object );

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false ).Verifiable();
			vm.Muter = muter.Object;
			vm.Dispatcher = new SyncDispatcher();

			// Act
			await vm.Load();

			// Assert
			Assert.AreEqual( 3, vm.Statuses.Count );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void LoadingMoreDataAddsStatuses()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig {RealtimeStreaming = false} );
			var parser = new Mock<IStreamParser>();

			var statuses = new List<Status>
			{
				DummyGenerator.CreateDummyStatus(),
				DummyGenerator.CreateDummyStatus(),
				DummyGenerator.CreateDummyStatus()
			};

			var waitHandle = new ManualResetEvent( false );

			var twitterContext = new Mock<ITwitterContext>();

			var statusRepo = new Mock<ITwitterStatusRepository>();
			statusRepo.Setup(
				s => s.Filter( It.IsAny<Expression<Func<Status, bool>>>(), It.IsAny<Expression<Func<Status, bool>>>() ) ).Returns(
					Task.FromResult( statuses ) );
			twitterContext.SetupGet( c => c.Statuses ).Returns( statusRepo.Object );
			context.Setup( c => c.Twitter ).Returns( twitterContext.Object );

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false ).Verifiable();
			vm.Muter = muter.Object;
			vm.Dispatcher = new SyncDispatcher();

			vm.NewStatus += ( s, e ) => waitHandle.Set();

			// Act
			vm.ActionDispatcher.OnBottomReached();
			waitHandle.WaitOne( 1000 );

			// Assert
			Assert.AreEqual( 3, vm.Statuses.Count );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void NewStatusCanBeRejectedByColumnType()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			bool checkCalled = false;
			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object )
			{
				SuitableCheck = s =>
				{
					checkCalled = true;
					return false;
				}
			};

			var status = new Status();

			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false ).Verifiable();
			vm.Muter = muter.Object;

			// Act
			parser.Raise( p => p.StatusReceived += null, new StatusStreamEventArgs( status ) );

			// Assert
			Assert.IsTrue( checkCalled );
			Assert.AreEqual( 0, vm.Statuses.Count );
			muter.Verify( m => m.IsMuted( It.IsAny<Status>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void NewStatusCanBeRejectedByMuter()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );

			var status = new Status();

			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( true ).Verifiable();
			vm.Muter = muter.Object;

			// Act
			parser.Raise( p => p.StatusReceived += null, new StatusStreamEventArgs( status ) );

			// Assert
			muter.Verify( m => m.IsMuted( It.IsAny<Status>() ), Times.Once() );
			Assert.AreEqual( 0, vm.Statuses.Count );
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
			vm.RaiseStatusWrapper( new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null ) );

			// Assert
			Assert.IsTrue( true ); // HACK: This is ugly...
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void NewStatusIsAddedToCollection()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			var status = DummyGenerator.CreateDummyStatus();

			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false ).Verifiable();
			vm.Muter = muter.Object;

			vm.Dispatcher = new SyncDispatcher();

			// Act
			parser.Raise( p => p.StatusReceived += null, new StatusStreamEventArgs( status ) );

			// Assert
			Assert.AreEqual( 1, vm.Statuses.Count );
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

			var status = new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null );

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
		public void NewStatusRaisesEvent()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			var status = DummyGenerator.CreateDummyStatus();

			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false ).Verifiable();
			vm.Muter = muter.Object;

			vm.Dispatcher = new SyncDispatcher();
			bool raised = false;
			vm.NewStatus += ( s, e ) => raised = true;
			vm.SetLoading( false );

			// Act
			parser.Raise( p => p.StatusReceived += null, new StatusStreamEventArgs( status ) );

			// Assert
			Assert.IsTrue( raised );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void NewStatusUpdatesCache()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			var status = DummyGenerator.CreateDummyStatus();
			status.Entities = new Entities
			{
				HashTagEntities = new List<HashTagEntity>
				{
					new HashTagEntity {Tag = "hashtag"}
				},
				UserMentionEntities = new List<UserMentionEntity>
				{
					new UserMentionEntity {ScreenName = "testi", Id = 123}
				}
			};

			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false ).Verifiable();
			vm.Muter = muter.Object;

			vm.Dispatcher = new SyncDispatcher();

			var cache = new Mock<IDataCache>();
			cache.Setup( c => c.AddUser( 123, "testi" ) ).Returns( Task.CompletedTask ).Verifiable();
			cache.Setup( c => c.AddHashtag( "tag" ) ).Returns( Task.CompletedTask ).Verifiable();

			vm.Cache = cache.Object;

			// Act
			parser.Raise( p => p.StatusReceived += null, new StatusStreamEventArgs( status ) );

			// Assert
			cache.Verify( c => c.AddUser( 123, "testi" ), Times.Once() );
			cache.Verify( c => c.AddHashtag( "hashtag" ), Times.Once() );
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
			tester.Test( nameof( ColumnViewModelBase.Muter ), nameof( ColumnViewModelBase.ContextList ),
				nameof( ColumnViewModelBase.ViewServiceRepository ),
				nameof( ColumnViewModelBase.Cache ), nameof( ColumnViewModelBase.Configuration ),
				nameof( ColumnViewModelBase.Dispatcher ),
				nameof( ColumnViewModelBase.ProcessStarter ) );

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void ReceivingFriendsStoresThemInCache()
		{
			// Arrange
			var userList = new List<User>
			{
				new User {UserID = 123, IncludeEntities = false, Type = UserType.Lookup, UserIdList = "123,456,789"},
				new User {UserID = 456, IncludeEntities = false, Type = UserType.Lookup, UserIdList = "123,456,789"},
				new User {UserID = 789, IncludeEntities = false, Type = UserType.Lookup, UserIdList = "123,456,789"}
			};

			var twitterContext = new Mock<ITwitterContext>();
			var userRepo = new Mock<ITwitterUserRepository>();
			userRepo.Setup( t => t.LookupUsers( "123,456,789" ) ).Returns( Task.FromResult( userList ) );
			twitterContext.SetupGet( c => c.Users ).Returns( userRepo.Object );

			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.Twitter ).Returns( twitterContext.Object );

			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();
			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );

			var cache = new Mock<IDataCache>();
			cache.Setup( c => c.AddUser( It.IsAny<ulong>(), It.IsAny<string>() ) ).Returns( Task.CompletedTask );
			vm.Cache = cache.Object;

			// Act
			parser.Raise( p => p.FriendsReceived += null, new FriendsStreamEventArgs( "{\"friends\":[123,456,789]}" ) );

			// Assert
			cache.Verify( c => c.AddUser( It.IsAny<ulong>(), It.IsAny<string>() ), Times.Exactly( 3 ) );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void RefereshingDataAddsStatuses()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig {RealtimeStreaming = false} );
			var parser = new Mock<IStreamParser>();

			var statuses = new List<Status>
			{
				DummyGenerator.CreateDummyStatus(),
				DummyGenerator.CreateDummyStatus(),
				DummyGenerator.CreateDummyStatus()
			};

			var waitHandle = new ManualResetEvent( false );

			var twitterContext = new Mock<ITwitterContext>();

			var statusRepo = new Mock<ITwitterStatusRepository>();
			statusRepo.Setup(
				s => s.Filter( It.IsAny<Expression<Func<Status, bool>>>(), It.IsAny<Expression<Func<Status, bool>>>() ) ).Returns(
					Task.FromResult( statuses ) );
			twitterContext.SetupGet( c => c.Statuses ).Returns( statusRepo.Object );
			context.Setup( c => c.Twitter ).Returns( twitterContext.Object );

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false ).Verifiable();
			vm.Muter = muter.Object;
			vm.Dispatcher = new SyncDispatcher();

			vm.NewStatus += ( s, e ) => waitHandle.Set();

			// Act
			vm.ActionDispatcher.OnHeaderClicked();
			waitHandle.WaitOne( 1000 );

			// Assert
			Assert.AreEqual( 3, vm.Statuses.Count );
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
				return SuitableCheck( status );
			}

			public override Icon Icon { get; }
			protected override Expression<Func<Status, bool>> StatusFilterExpression { get; }
			public Func<Status, bool> SuitableCheck = s => true;
		}
	}
}