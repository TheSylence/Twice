using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Twitter;
using Twice.ViewModels.Dialogs;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Dialogs
{
	[ExcludeFromCodeCoverage, TestClass]
	public class RetweetDialogViewModelTests
	{
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

			vm.Accounts.Add( new AccountEntry( context.Object ) );
			bool withUnselectedAccount = vm.QuoteCommand.CanExecute( null );

			vm.Accounts.First().Use = true;
			bool withSelectedAccount = vm.QuoteCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( withoutAccount );
			Assert.IsFalse( withUnselectedAccount );
			Assert.IsTrue( withSelectedAccount );
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

			vm.Accounts.Add( new AccountEntry( context.Object ) );
			bool withUnselectedAccount = vm.RetweetCommand.CanExecute( null );

			vm.Accounts.First().Use = true;
			bool withSelectedAccount = vm.RetweetCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( withoutAccount );
			Assert.IsFalse( withUnselectedAccount );
			Assert.IsTrue( withSelectedAccount );
		}
	}
}