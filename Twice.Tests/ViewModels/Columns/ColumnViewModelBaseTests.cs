using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
using Twice.Views.Services;

namespace Twice.Tests.ViewModels.Columns
{
	[TestClass, ExcludeFromCodeCoverage]
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
			vm.Items.Add( new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null, null ) );
			vm.Items.Add( new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null, null ) );
			vm.Items.Add( new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null, null ) );

			// Act
			int countBefore = vm.Items.Count;
			vm.ClearCommand.Execute( null );
			int countAfter = vm.Items.Count;

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

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.Confirm( It.IsAny<ConfirmServiceArgs>() ) ).Returns( Task.FromResult( true ) );

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			bool raised = false;
			vm.Deleted += ( s, e ) => raised = true;
			vm.ViewServiceRepository = viewServices.Object;

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

			for( int i = 0; i < statuses.Count; ++i )
			{
				statuses[i].StatusID = (ulong)( i + 1 );
			}

			var twitterContext = new Mock<ITwitterContext>();

			var statusRepo = new Mock<ITwitterStatusRepository>();
			statusRepo.Setup( s => s.Filter(
				It.IsAny<Expression<Func<Status, bool>>>(),
				It.IsAny<Expression<Func<Status, bool>>>() ) ).Returns( Task.FromResult( statuses ) );
			twitterContext.SetupGet( c => c.Statuses ).Returns( statusRepo.Object );
			context.Setup( c => c.Twitter ).Returns( twitterContext.Object );

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false ).Verifiable();
			vm.Muter = muter.Object;
			vm.Dispatcher = new SyncDispatcher();
			vm.Cache = new NullCache();

			// Act
			await vm.Load();

			// Assert
			Assert.AreEqual( 3, vm.Items.Count );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void ItemsAreSortedBasedOnId()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();
			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false );

			var column = new TestColumn( context.Object, new ColumnDefinition( ColumnType.Timeline ), config.Object,
				parser.Object )
			{
				Dispatcher = new SyncDispatcher(),
				Muter = muter.Object
			};
			column.SetLoading( false );

			var s1 = DummyGenerator.CreateDummyStatus();
			s1.ID = s1.StatusID = 1;
			var s2 = DummyGenerator.CreateDummyStatus();
			s2.ID = s2.StatusID = 2;
			var s3 = DummyGenerator.CreateDummyStatus();
			s3.ID = s3.StatusID = 3;

			var waitHandle = new ManualResetEventSlim( false );
			column.NewItem += ( s, e ) => { waitHandle.Set(); };

			// Act
			parser.Raise( p => p.StatusReceived += null, new StatusStreamEventArgs( s2 ) );
			bool set1 = waitHandle.Wait( 1000 );
			waitHandle.Reset();

			parser.Raise( p => p.StatusReceived += null, new StatusStreamEventArgs( s3 ) );
			bool set2 = waitHandle.Wait( 1000 );
			waitHandle.Reset();

			parser.Raise( p => p.StatusReceived += null, new StatusStreamEventArgs( s1 ) );
			bool set3 = waitHandle.Wait( 1000 );

			// Assert
			Assert.IsTrue( set1 );
			Assert.IsTrue( set2 );
			Assert.IsTrue( set3 );

			var ids = column.Items.Select( it => it.Id ).ToArray();
			CollectionAssert.AreEqual( new ulong[] {3, 2, 1}, ids );
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

			for( int i = 0; i < statuses.Count; ++i )
			{
				statuses[i].StatusID = (ulong)( i + 1 );
			}

			var waitHandle = new ManualResetEventSlim( false );

			var twitterContext = new Mock<ITwitterContext>();

			var statusRepo = new Mock<ITwitterStatusRepository>();
			statusRepo.Setup(
				s =>
					s.Filter( It.IsAny<Expression<Func<Status, bool>>>(), It.IsAny<Expression<Func<Status, bool>>>(),
						It.IsAny<Expression<Func<Status, bool>>>() ) ).Returns(
				Task.FromResult( statuses ) );
			twitterContext.SetupGet( c => c.Statuses ).Returns( statusRepo.Object );
			context.Setup( c => c.Twitter ).Returns( twitterContext.Object );

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false ).Verifiable();
			vm.Muter = muter.Object;
			vm.Dispatcher = new SyncDispatcher();
			vm.Cache = new NullCache();

			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( ColumnViewModelBase.IsLoading ) && vm.IsLoading == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.ActionDispatcher.OnBottomReached();
			bool wasSet = waitHandle.Wait( 1000 );

			// Assert
			Assert.IsTrue( wasSet );
			Assert.AreEqual( 3, vm.Items.Count );
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
			Assert.AreEqual( 0, vm.Items.Count );
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
			Assert.AreEqual( 0, vm.Items.Count );
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
			vm.RaiseStatusWrapper( new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null, null ) );

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
			Assert.AreEqual( 1, vm.Items.Count );
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
			vm.NewItem += ( s, e ) => raised = true;

			var status = new StatusViewModel( DummyGenerator.CreateDummyStatus(), context.Object, null, null );

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
			vm.NewItem += ( s, e ) => raised = true;
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

			var cache = new Mock<ICache>();
			cache.Setup( c => c.AddUsers( It.Is<IList<UserCacheEntry>>( ca => ca[0].UserId == 123 ) ) ).Returns(
					Task.CompletedTask )
				.Verifiable();
			cache.Setup( c => c.AddHashtags( new[] {"hashtag"} ) ).Returns( Task.CompletedTask ).Verifiable();

			vm.Cache = cache.Object;

			// Act
			parser.Raise( p => p.StatusReceived += null, new StatusStreamEventArgs( status ) );

			// Assert
			cache.Verify( c => c.AddUsers( It.Is<IList<UserCacheEntry>>( ca => ca[0].UserId == 123 ) ), Times.Once() );
			cache.Verify( c => c.AddHashtags( new[] {"hashtag"} ), Times.Once() );
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
			var tester = new PropertyChangedTester( vm, true );

			// Act
			tester.Test( nameof( ColumnViewModelBase.Muter ), nameof( ColumnViewModelBase.Cache ) );

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void ParserDeletionDoesNotCrashIfStatusIsContainedMultipleTimes()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();
			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object )
			{
				Dispatcher = new SyncDispatcher()
			};

			var status = DummyGenerator.CreateDummyStatus();
			status.StatusID = status.ID = 1234;
			var s1 = new StatusViewModel( status, context.Object, config.Object, null );
			vm.Items.Add( s1 );

			status = DummyGenerator.CreateDummyStatus();
			status.StatusID = status.ID = 1234;
			var s2 = new StatusViewModel( status, context.Object, config.Object, null );
			vm.Items.Add( s2 );

			var deleteArgs =
				new DeleteStreamEventArgs(
					"{\"delete\":{\"status\":{\"id\":1234,\"id_str\":\"1234\",\"user_id\":3,\"user_id_str\":\"3\"}}}" );

			// Act
			parser.Raise( e => e.StatusDeleted += null, deleteArgs );

			// Assert
			Assert.IsFalse( vm.Items.Any( s => s.Id == 1234 ) );
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

			for( int i = 0; i < statuses.Count; ++i )
			{
				statuses[i].StatusID = (ulong)( i + 1 );
			}

			var waitHandle = new ManualResetEventSlim( false );

			var twitterContext = new Mock<ITwitterContext>();

			var statusRepo = new Mock<ITwitterStatusRepository>();
			statusRepo.Setup(
				s =>
					s.Filter( It.IsAny<Expression<Func<Status, bool>>>(), It.IsAny<Expression<Func<Status, bool>>>(),
						It.IsAny<Expression<Func<Status, bool>>>() ) ).Returns(
				Task.FromResult( statuses ) );
			twitterContext.SetupGet( c => c.Statuses ).Returns( statusRepo.Object );
			context.Setup( c => c.Twitter ).Returns( twitterContext.Object );

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object );
			var muter = new Mock<IStatusMuter>();
			muter.Setup( m => m.IsMuted( It.IsAny<Status>() ) ).Returns( false ).Verifiable();
			vm.Muter = muter.Object;
			vm.Dispatcher = new SyncDispatcher();
			vm.Cache = new NullCache();

			vm.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( ColumnViewModelBase.IsLoading ) && vm.IsLoading == false )
				{
					waitHandle.Set();
				}
			};

			// Act
			vm.ActionDispatcher.OnHeaderClicked();
			bool wasSet = waitHandle.Wait( 1000 );

			// Assert
			Assert.IsTrue( wasSet );
			Assert.AreEqual( 3, vm.Items.Count );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public void RelativeTimesAreUpdated()
		{
			// Arrange
			var item = new Mock<ColumnItem>();
			item.Setup( it => it.UpdateRelativeTime() ).Verifiable();

			var vm = new TestColumn();
			vm.Items.Add( item.Object );

			// Act
			vm.UpdateRelativeTimes();

			// Assert
			item.Verify( it => it.UpdateRelativeTime(), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Columns" )]
		public async Task RetweetsAreCorrectlyOrdered()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			var definition = new ColumnDefinition( ColumnType.User );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var parser = new Mock<IStreamParser>();

			var vm = new TestColumn( context.Object, definition, config.Object, parser.Object )
			{
				Dispatcher = new SyncDispatcher()
			};

			var s1 = DummyGenerator.CreateDummyStatus();
			s1.ID = s1.StatusID = 1;
			s1.Text = "s1";
			var s2 = DummyGenerator.CreateDummyStatus();
			s2.ID = s2.StatusID = 2;
			s2.Text = "s2";
			var s3 = DummyGenerator.CreateDummyStatus();
			s3.RetweetedStatus = s1;
			s3.ID = s3.StatusID = 3;
			s3.Text = "s1";

			// Act
			await vm.Add( new StatusViewModel( s2, context.Object, null, null ) );
			await vm.Add( new StatusViewModel( s3, context.Object, null, null ) );

			// Assert
			Assert.AreEqual( 2, vm.Items.Count );

			Assert.AreEqual( "s1", vm.Items.First().Text );
			Assert.AreEqual( "s2", vm.Items.Last().Text );
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
			public TestColumn( IContextEntry context, ColumnDefinition definition, IConfig config = null, IStreamParser parser = null )
				: base( context, definition, config ?? DefaultConfig(), parser ?? DefaultParser() )
			{
				StatusFilterExpression = s => true;
				Icon = Icon.User;
			}

			public TestColumn()
				: base( DefaultContext(), new ColumnDefinition( ColumnType.User ), DefaultConfig(), DefaultParser() )
			{
			}

			public Task Add( StatusViewModel status )
			{
				return AddItem( status );
			}

			public void RaiseStatusWrapper( ColumnItem iteme )
			{
				RaiseNewItem( iteme );
			}

			public void SetLoading( bool isLoading )
			{
				IsLoading = isLoading;
			}

			protected override bool IsSuitableForColumn( Status status )
			{
				return SuitableCheck( status );
			}

			protected override bool IsSuitableForColumn( DirectMessage message )
			{
				return false;
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

			public override Icon Icon { get; }
			protected override Expression<Func<Status, bool>> StatusFilterExpression { get; }
			public Func<Status, bool> SuitableCheck = s => true;
		}
	}
}