using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using Twice.Utilities.Os;
using Twice.ViewModels.Dialogs;

namespace Twice.Tests.ViewModels.Dialogs
{
	[TestClass, ExcludeFromCodeCoverage]
	public class ImageDialogViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Dialogs" )]
		public void CopyCommandWritesToClipboard()
		{
			// Arrange
			var clipboard = new Mock<IClipboard>();
			clipboard.Setup( c => c.SetText( "https://example.com/link.url" ) ).Verifiable();

			var vm = new ImageDialogViewModel
			{
				Clipboard = clipboard.Object,
				SelectedImage = new ImageEntry( new Uri( "https://example.com/link.url" ), false, "ImageTitle" )
			};

			// Act
			vm.CopyToClipboardCommand.Execute( null );

			// Assert
			clipboard.Verify( c => c.SetText( "https://example.com/link.url" ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Dialogs" )]
		public void ImagesCanBeSet()
		{
			// Arrange
			var vm = new ImageDialogViewModel();

			var urls = new[]
			{
				new Uri( "http://example.com/1.png" ),
				new Uri( "http://example.com/2.png" ),
				new Uri( "http://example.com/3.png" )
			};

			// Act
			vm.SetImages( urls );

			// Assert
			Assert.AreEqual( 3, vm.Images.Count );
		}

		[TestMethod, TestCategory( "ViewModels.Dialogs" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var resolver = new Mock<ITypeResolver>();
			resolver.Setup( r => r.Resolve( typeof( ImageEntry ) ) ).Returns( new ImageEntry( new Uri( "http://example.com" ), false ) );

			var vm = new ImageDialogViewModel();
			var tester = new PropertyChangedTester( vm, false, resolver.Object );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Dialog" )]
		public void OpenCommandExecutesUrl()
		{
			// Arrange
			var procStarter = new Mock<IProcessStarter>();
			procStarter.Setup( c => c.Start( "https://example.com/link.url" ) ).Verifiable();

			var vm = new ImageDialogViewModel
			{
				ProcessStarter = procStarter.Object,
				SelectedImage = new ImageEntry( new Uri( "https://example.com/link.url" ), false, "ImageTitle" )
			};

			// Act
			vm.OpenImageCommand.Execute( null );

			// Assert
			procStarter.Verify( c => c.Start( "https://example.com/link.url" ), Times.Once() );
		}
	}
}