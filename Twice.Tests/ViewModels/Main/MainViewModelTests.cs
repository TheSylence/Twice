using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.ViewModels;
using Twice.ViewModels.Columns;
using Twice.ViewModels.Main;
using Twice.ViewModels.Twitter;
using Twice.Views.Services;

namespace Twice.Tests.ViewModels.Main
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MainViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void AccountCommandOpensDialog()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ShowAccounts( false ) ).Returns( Task.CompletedTask );
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.AccountsCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ShowAccounts( false ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task AccountDialogIsOpenedIfUserWantsToAddAccountWhenNoneExists()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new IContextEntry[0] );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.Confirm( It.IsAny<ConfirmServiceArgs>() ) )
				.Returns( Task.FromResult( true ) );
			viewServices.Setup( v => v.ShowAccounts( true ) ).Returns( Task.CompletedTask ).Verifiable();

			vm.ViewServiceRepository = viewServices.Object;

			// Act
			await vm.OnLoad( null );

			// Assert
			viewServices.Verify( v => v.ShowAccounts( true ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void AddColumnCommandNeedsAccount()
		{
			// Arrange
			var contexts = new List<IContextEntry>();

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( () => contexts );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			// Act
			bool withoutAccount = vm.AddColumnCommand.CanExecute( null );
			contexts.Add( new Mock<IContextEntry>().Object );
			bool withAccount = vm.AddColumnCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( withoutAccount );
			Assert.IsTrue( withAccount );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void AddColumnCommandOpensDialog()
		{
			// Arrange
			var contexts = new[] { new Mock<IContextEntry>().Object };
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( contexts );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ShowAddColumnDialog() ).Returns( Task.CompletedTask );
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.AddColumnCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ShowAddColumnDialog(), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void ChangingAccountsInformsColumnList()
		{
			// Arrange
			var ctx1 = new Mock<IContextEntry>();
			ctx1.SetupGet( c => c.UserId ).Returns( 123 );
			var ctx2 = new Mock<IContextEntry>();
			ctx2.SetupGet( c => c.UserId ).Returns( 111 );

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[]
			{
				ctx1.Object, ctx2.Object
			} );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();

			// ReSharper disable PossibleMultipleEnumeration
			Expression<Func<IEnumerable<ulong>, bool>> checkAction = ids => ids.Contains( 123ul ) && ids.Contains( 111ul );

			// ReSharper restore PossibleMultipleEnumeration

			columnList.Setup( c => c.SetExistingContexts( It.Is( checkAction ) ) ).Verifiable();
			var columnFactory = new Mock<IColumnFactory>();

			// Act ReSharper disable once UnusedVariable
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );
			contextList.Raise( ctx => ctx.ContextsChanged += null, EventArgs.Empty );

			// Assert
			columnList.Verify( c => c.SetExistingContexts( It.Is( checkAction ) ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void ColumnEventsAreSubscribedTo()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.OnItem( It.IsAny<ColumnItem>(), It.IsAny<ColumnNotifications>() ) ).Verifiable();

			var columnList = new Mock<IColumnDefinitionList>();
			columnList.Setup( c => c.Load() ).Returns( new[] { new ColumnDefinition( ColumnType.DebugOrTest ) } );
			columnList.Setup( c => c.Update( It.IsAny<IEnumerable<ColumnDefinition>>() ) ).Verifiable();
			columnList.Setup( c => c.Remove( It.IsAny<IEnumerable<ColumnDefinition>>() ) ).Verifiable();

			var column = new Mock<TestColumnMock>();

			Expression<Func<ColumnDefinition, bool>> columnVerifier = col => col.Type == ColumnType.DebugOrTest;
			var columnFactory = new Mock<IColumnFactory>();
			columnFactory.Setup( c => c.Construct( It.Is( columnVerifier ) ) ).Returns( column.Object );

			// ReSharper disable once UnusedVariable
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			// Act
			column.Raise( c => c.Changed += null, EventArgs.Empty );
			column.Raise( c => c.NewItem += null, new ColumnItemEventArgs( null ) );
			column.Raise( c => c.Deleted += null, EventArgs.Empty );

			// Assert
			columnList.Verify( c => c.Update( It.IsAny<IEnumerable<ColumnDefinition>>() ), Times.Once() );
			columnList.Verify( c => c.Remove( It.IsAny<IEnumerable<ColumnDefinition>>() ), Times.Once() );
			notifier.Verify( n => n.OnItem( It.IsAny<ColumnItem>(), It.IsAny<ColumnNotifications>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task ColumnsAreLoadedOnLoad()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.LogCurrentRateLimits() ).Returns( Task.CompletedTask );
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] { context.Object } );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var column = new Mock<IColumnViewModel>();
			column.Setup( c => c.Load( AsyncLoadContext.Ui ) ).Returns( Task.CompletedTask ).Verifiable();
			vm.Columns.Add( column.Object );
			vm.Columns.Add( column.Object );

			// Act
			await vm.OnLoad( null );

			// Assert
			column.Verify( c => c.Load( AsyncLoadContext.Ui ), Times.Exactly( 2 ) );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task ExceptionOnCredentialValidationDoesNotCrash()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.VerifyCredentials() ).Throws(
				new WebException( "The remote name could not be resolved: 'api.twitter.com'" ) )
				.Verifiable();

			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] { context.Object } );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var vm = new MainViewModel( contextList.Object, null, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			// Act
			await vm.CheckCredentials();

			// Assert
			context.Verify( c => c.Twitter.VerifyCredentials(), Times.AtLeastOnce() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void InfoCommandOpensAboutDialog()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ShowInfo() ).Returns( Task.CompletedTask );
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.InfoCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ShowInfo(), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task MissingUpdateExeDoesNotCrashUpdateCheck()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.LogCurrentRateLimits() ).Returns( Task.CompletedTask );
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] { context.Object } );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();
			var generalCfg = new GeneralConfig { CheckForUpdates = true };
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( generalCfg );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var updaterFactory = new Mock<IAppUpdaterFactory>();
			updaterFactory.Setup( f => f.Construct( It.IsAny<bool>() ) ).Throws( new Exception( "Update.exe not found" ) );

			vm.UpdateFactory = updaterFactory.Object;

			// Act
			var ex = await ExceptionAssert.CatchAsync<Exception>( () => vm.OnLoad( null ) );

			// Assert
			Assert.IsNull( ex );

			updaterFactory.Verify( f => f.Construct( It.IsAny<bool>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void NewTweetCommandNeedsAccount()
		{
			// Arrange
			var contexts = new List<IContextEntry>();

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( () => contexts );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			// Act
			bool withoutAccount = vm.NewTweetCommand.CanExecute( null );
			contexts.Add( new Mock<IContextEntry>().Object );
			bool withAccount = vm.NewTweetCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( withoutAccount );
			Assert.IsTrue( withAccount );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void NewTweetCommandOpensWindow()
		{
			// Arrange
			var contexts = new[] { new Mock<IContextEntry>().Object };
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( contexts );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ComposeTweet( null ) ).Returns( Task.CompletedTask ).Verifiable();
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.NewTweetCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ComposeTweet( null ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void SettingsCommandsOpensDialog()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ShowSettings() ).Returns( Task.CompletedTask );
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.SettingsCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ShowSettings(), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task UpdateCheckIsDoneWithCorrectChannel()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.LogCurrentRateLimits() ).Returns( Task.CompletedTask );
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] { context.Object } );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();
			var generalCfg = new GeneralConfig();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( generalCfg );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var updater = new Mock<IAppUpdater>();
			var updaterFactory = new Mock<IAppUpdaterFactory>( MockBehavior.Strict );
			updaterFactory.Setup( f => f.Construct( true ) ).Returns( Task.FromResult( updater.Object ) );
			updaterFactory.Setup( f => f.Construct( false ) ).Returns( Task.FromResult( updater.Object ) );

			vm.UpdateFactory = updaterFactory.Object;
			vm.Configuration = config.Object;

			// Act
			generalCfg.CheckForUpdates = false;
			await vm.OnLoad( null );

			generalCfg.CheckForUpdates = true;
			generalCfg.IncludePrereleaseUpdates = false;
			await vm.OnLoad( null );

			generalCfg.CheckForUpdates = true;
			generalCfg.IncludePrereleaseUpdates = true;
			await vm.OnLoad( null );

			// Assert
			updaterFactory.Verify( f => f.Construct( true ), Times.Once() );
			updaterFactory.Verify( f => f.Construct( false ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task UserIsAskedToAddAccountIfNoneExists()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new IContextEntry[0] );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.Confirm( It.IsAny<ConfirmServiceArgs>() ) )
				.Returns( Task.FromResult( false ) ).Verifiable();

			vm.ViewServiceRepository = viewServices.Object;

			// Act
			await vm.OnLoad( null );

			// Assert
			viewServices.Verify( v => v.Confirm( It.IsAny<ConfirmServiceArgs>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task UserIsInformedAboutUpdatedVersion()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.LogCurrentRateLimits() ).Returns( Task.CompletedTask );
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] { context.Object } );
			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.DisplayMessage( It.IsAny<string>(), NotificationType.Information ) ).Verifiable();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();
			var generalCfg = new GeneralConfig { CheckForUpdates = true };
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( generalCfg );
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var version = new SemanticVersion( 999, 9, 9, 9 );
			var updater = new Mock<IAppUpdater>();
			updater.Setup( u => u.UpdateApp() ).Returns( Task.FromResult( new AppRelease( version ) ) );

			var updaterFactory = new Mock<IAppUpdaterFactory>();
			updaterFactory.Setup( f => f.Construct( It.IsAny<bool>() ) ).Returns( Task.FromResult( updater.Object ) );
			vm.UpdateFactory = updaterFactory.Object;

			vm.Configuration = config.Object;

			// Act
			await vm.OnLoad( null );

			// Assert
			notifier.Verify( n => n.DisplayMessage( It.IsAny<string>(), NotificationType.Information ), Times.Once() );
		}
	}
}