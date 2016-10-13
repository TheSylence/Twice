using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twice.Models.Twitter;
using Twice.ViewModels;
using Twice.ViewModels.Dialogs;
using Twice.ViewModels.Twitter;
using Twice.Views.Services;

namespace Twice.Tests.ViewModels.Dialogs
{
	[ExcludeFromCodeCoverage, TestClass]
	public class RetweetDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Dialogs" )]
		public async Task AccountsAreCorrectlyLoaded()
		{
			// Arrange
			var vm = new RetweetDialogViewModel();
			var c1 = new Mock<IContextEntry>();
			c1.Setup( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );
			var c2 = new Mock<IContextEntry>();
			c2.Setup( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[]
			{
				c1.Object, c2.Object
			} );

			vm.ContextList = contextList.Object;

			// Act
			await vm.OnLoad( false );

			// Assert
			Assert.AreEqual( 2, vm.Accounts.Count );
		}

		[TestMethod, TestCategory( "ViewModels.Dialogs" )]
		public void ConfirmationMustBeSetOnAccount()
		{
			// Arrange
			var status = new StatusViewModel( DummyGenerator.CreateDummyStatus(), null, null, null );

			var vm = new RetweetDialogViewModel
			{
				Status = status
			};

			var context = new Mock<IContextEntry>();
			context.Setup( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );
			var context2 = new Mock<IContextEntry>();
			context2.Setup( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );
			context2.SetupGet( c => c.RequiresConfirmation ).Returns( true );

			// Act
			vm.Accounts.Add( new AccountEntry( context.Object, false ) );
			bool noAccount = vm.ConfirmationRequired;

			vm.Accounts.First().Use = true;
			bool oneAccount = vm.ConfirmationRequired;

			vm.Accounts.Add( new AccountEntry( context2.Object, false ) {Use = true} );
			bool requiredAccount = vm.ConfirmationRequired;

			// Assert
			Assert.IsFalse( noAccount );
			Assert.IsFalse( oneAccount );
			Assert.IsTrue( requiredAccount );
		}

		[TestMethod, TestCategory( "ViewModels.Dialogs" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var vm = new RetweetDialogViewModel();
			var typeResolver = new Mock<ITypeResolver>();
			typeResolver.Setup( t => t.Resolve( typeof( StatusViewModel ) ) ).Returns( new StatusViewModel( DummyGenerator.CreateDummyStatus(), null, null, null ) );
			var tester = new PropertyChangedTester( vm, false, typeResolver.Object );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Dialogs" )]
		public void QuoteCommandNeedsSelectedAccount()
		{
			// Arrange
			var vm = new RetweetDialogViewModel();
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );

			// Act
			bool withoutAccount = vm.QuoteCommand.CanExecute( null );

			vm.Accounts.Add( new AccountEntry( context.Object, false ) );
			bool withUnselectedAccount = vm.QuoteCommand.CanExecute( null );

			vm.Accounts.First().Use = true;
			bool withSelectedAccount = vm.QuoteCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( withoutAccount );
			Assert.IsFalse( withUnselectedAccount );
			Assert.IsTrue( withSelectedAccount );
		}

		[TestMethod, TestCategory( "ViewModels.Dialogs" )]
		public void QuoteCommandOpensDialog()
		{
			// Arrange
			var statusVm = new StatusViewModel( DummyGenerator.CreateDummyStatus(), null, null, null );
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.UserId ).Returns( 1 );
			context.Setup( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );

			var context2 = new Mock<IContextEntry>();
			context2.SetupGet( c => c.UserId ).Returns( 2 );
			context2.Setup( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.QuoteTweet( statusVm, It.IsAny<IEnumerable<ulong>>() ) ).Returns( Task.CompletedTask ).Verifiable();

			var vm = new RetweetDialogViewModel
			{
				Status = statusVm,
				ViewServiceRepository = viewServices.Object
			};
			vm.Accounts.Add( new AccountEntry( context.Object, false ) );
			vm.Accounts.Add( new AccountEntry( context2.Object, false ) );
			vm.Accounts.First().Use = true;

			// Act
			vm.QuoteCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.QuoteTweet( statusVm, It.IsAny<IEnumerable<ulong>>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Dialog" )]
		public void RetweetCommandNeedsSelectedAccount()
		{
			// Arrange
			var vm = new RetweetDialogViewModel();
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );

			// Act
			bool withoutAccount = vm.RetweetCommand.CanExecute( null );

			vm.Accounts.Add( new AccountEntry( context.Object, false ) );
			bool withUnselectedAccount = vm.RetweetCommand.CanExecute( null );

			vm.Accounts.First().Use = true;
			bool withSelectedAccount = vm.RetweetCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( withoutAccount );
			Assert.IsFalse( withUnselectedAccount );
			Assert.IsTrue( withSelectedAccount );
		}

		[TestMethod, TestCategory( "ViewModels.Dialog" )]
		public async Task RetweetIsExecutedImmediatlyWithOneAccount()
		{
			// Arrange
			var vm = new RetweetDialogViewModel();
			var contextList = new Mock<ITwitterContextList>();
			var context = new Mock<IContextEntry>();
			context.SetupGet( c => c.Notifier ).Returns( new Mock<INotifier>().Object );
			context.Setup( c => c.Twitter.Statuses.RetweetAsync( 123ul ) ).Returns( Task.FromResult( new LinqToTwitter.Status() ) ).Verifiable();
			context.Setup( c => c.ProfileImageUrl ).Returns( new System.Uri( "http://example.com/image.png" ) );
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} );

			var notifyHandle = new ManualResetEventSlim( false );
			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.DisplayMessage( It.IsAny<string>(), NotificationType.Success ) ).Callback( () => { notifyHandle.Set(); } );

			context.SetupGet( c => c.Notifier ).Returns( notifier.Object );

			var status = DummyGenerator.CreateDummyStatus();
			status.ID = 123ul;
			var statusVm = new StatusViewModel( status, context.Object, null, null )
			{
				Dispatcher = new SyncDispatcher()
			};

			vm.ContextList = contextList.Object;
			vm.Status = statusVm;
			vm.Dispatcher = new SyncDispatcher();

			var waitHandle = new ManualResetEventSlim( false );

			bool closed = false;
			vm.CloseRequested += ( s, e ) =>
			{
				closed = true;
				waitHandle.Set();
			};

			// Act
			await vm.OnLoad( false );
			bool set = waitHandle.Wait( 1000 ) && notifyHandle.Wait( 1000 );

			// Assert
			Assert.IsTrue( set, "WaitHandle not set" );
			Assert.IsTrue( closed );
			context.Verify( c => c.Twitter.Statuses.RetweetAsync( 123ul ), Times.Once() );
		}
	}
}