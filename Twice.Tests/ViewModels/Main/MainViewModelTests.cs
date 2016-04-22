using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twice.Messages;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Services.Views;
using Twice.ViewModels;
using Twice.ViewModels.Columns;
using Twice.ViewModels.Main;
using Twice.Views;

namespace Twice.Tests.ViewModels.Main
{
	[TestClass]
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
		public void NewTweetCommandSendsFlyoutMessage()
		{
			// Arrange
			var contexts = new[] { new Mock<IContextEntry>().Object };
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( contexts );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var messenger = new Mock<IMessenger>();
			messenger.Setup( m => m.Send( It.Is<FlyoutMessage>( msg => msg.Name == FlyoutNames.TweetComposer && msg.Action == FlyoutAction.Open) ) ).Verifiable();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object, messenger.Object );

			// Act
			vm.NewTweetCommand.Execute( null );

			// Assert
			messenger.Verify( m => m.Send( It.Is<FlyoutMessage>( msg => msg.Name == FlyoutNames.TweetComposer && msg.Action == FlyoutAction.Open ) ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void SettingsCommandsOpensDialog()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ShowSettings() ).Returns( Task.CompletedTask );
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.SettingsCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ShowSettings(), Times.Once() );
		}
	}
}