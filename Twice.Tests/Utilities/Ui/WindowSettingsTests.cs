using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using Twice.Utilities.Ui;
using Twice.Views;

namespace Twice.Tests.Utilities.Ui
{
	[TestClass, ExcludeFromCodeCoverage]
	public class WindowSettingsTests
	{
		[TestMethod, TestCategory( "Utilities.Ui" )]
		public void LoadingFromNonExistingFileReturnsNull()
		{
			// Act
			var loaded = WindowSettings.Load( "non.existing" );

			// Assert
			Assert.IsNull( loaded );
		}

		[TestMethod, TestCategory( "Utilities.Ui" )]
		public void SavingMinimizedWindowStateDoesNothing()
		{
			// Arrange
			var window = new Mock<IWindowAdapter>();
			window.SetupGet( w => w.WindowState ).Returns( WindowState.Minimized );

			// Act
			bool saved = new WindowSettings().Save( window.Object );

			// Assert
			Assert.IsFalse( saved );
		}

		[TestMethod, TestCategory( "Utilities.Ui" )]
		public void SettingsAreCorrectlyApplied()
		{
			// Arrange
			var settings = new WindowSettings
			{
				Width = 1,
				Height = 2,
				Left = 3,
				Top = 4,
				State = WindowState.Maximized
			};

			var window = new Mock<IWindowAdapter>();
			window.SetupSet( w => w.Width = 1.0 ).Verifiable();
			window.SetupSet( w => w.Height = 2.0 ).Verifiable();
			window.SetupSet( w => w.Left = 3.0 ).Verifiable();
			window.SetupSet( w => w.Top = 4.0 ).Verifiable();
			window.SetupSet( w => w.WindowState = WindowState.Maximized ).Verifiable();

			// Act
			settings.Apply( window.Object );

			// Assert
			window.VerifySet( w => w.Width = 1.0, Times.Once() );
			window.VerifySet( w => w.Height = 2.0, Times.Once() );
			window.VerifySet( w => w.Left = 3.0, Times.Once() );
			window.VerifySet( w => w.Top = 4.0, Times.Once() );
			window.VerifySet( w => w.WindowState = WindowState.Maximized, Times.Once() );
		}

		[TestMethod, TestCategory( "Utilities.Ui" )]
		public void SettingsAreCorrectlyLoaded()
		{
			// Arrange
			const string fileName = "WindowSettingsTests.SettingsAreCorrectlyLoaded";

			var settings = new WindowSettings
			{
				Width = 1,
				Height = 2,
				Left = 3,
				Top = 4,
				State = WindowState.Maximized
			};

			File.WriteAllText( fileName, JsonConvert.SerializeObject( settings ) );

			// Act
			var loaded = WindowSettings.Load( fileName );

			// Assert
			Assert.AreEqual( 1.0, loaded.Width );
			Assert.AreEqual( 2.0, loaded.Height );
			Assert.AreEqual( 3.0, loaded.Left );
			Assert.AreEqual( 4.0, loaded.Top );
			Assert.AreEqual( WindowState.Maximized, loaded.State );
		}

		[TestMethod, TestCategory( "Utilities.Ui" )]
		public void SettingsAreCorrectlySaved()
		{
			// Arrange
			const string fileName = "WindowSettingsTests.SettingsAreCorrectlySaved";

			var window = new Mock<IWindowAdapter>();
			window.SetupGet( w => w.Height ).Returns( 1 );
			window.SetupGet( w => w.Width ).Returns( 2 );
			window.SetupGet( w => w.Top ).Returns( 3 );
			window.SetupGet( w => w.Left ).Returns( 4 );
			window.SetupGet( w => w.WindowState ).Returns( WindowState.Maximized );

			var settings = new WindowSettings();

			// Act
			bool saved = settings.Save( fileName, window.Object );

			// Assert
			Assert.IsTrue( saved );

			var json = File.ReadAllText( fileName );
			var loaded = JsonConvert.DeserializeObject<WindowSettings>( json );

			Assert.AreEqual( 1.0, loaded.Height );
			Assert.AreEqual( 2.0, loaded.Width );
			Assert.AreEqual( 3.0, loaded.Top );
			Assert.AreEqual( 4.0, loaded.Left );
			Assert.AreEqual( WindowState.Maximized, loaded.State );
		}

		[TestMethod, TestCategory( "Utilities.Ui" )]
		public void WindowIsMovedLeftOnScreenIfNeeded()
		{
			// Arrange
			var screen = new Mock<IVirtualScreen>();
			screen.SetupGet( s => s.Width ).Returns( 500 );
			screen.SetupGet( s => s.Height ).Returns( 400 );

			var settings = new WindowSettings
			{
				Width = 250,
				Height = 200,
				Left = 700,
				VirtualScreen = screen.Object
			};

			var window = new Mock<IWindowAdapter>();
			window.SetupSet( w => w.Left = 250 ).Verifiable();

			// Act
			settings.Apply( window.Object );

			// Assert
			window.VerifySet( w => w.Left = 250, Times.Once() );
		}

		[TestMethod, TestCategory( "Utilities.Ui" )]
		public void WindowIsMovedTopOnScreenIfNeeded()
		{
			// Arrange
			var screen = new Mock<IVirtualScreen>();
			screen.SetupGet( s => s.Width ).Returns( 500 );
			screen.SetupGet( s => s.Height ).Returns( 400 );

			var settings = new WindowSettings
			{
				Width = 250,
				Height = 200,
				Left = 0,
				Top = 700,
				VirtualScreen = screen.Object
			};

			var window = new Mock<IWindowAdapter>();
			window.SetupSet( w => w.Top = 200 ).Verifiable();

			// Act
			settings.Apply( window.Object );

			// Assert
			window.VerifySet( w => w.Top = 200, Times.Once() );
		}

		[TestMethod, TestCategory( "Utilities.Ui" )]
		public void WindowSizeIsClampedToScreenSize()
		{
			// Arrange
			var screen = new Mock<IVirtualScreen>();
			screen.SetupGet( s => s.Width ).Returns( 500 );
			screen.SetupGet( s => s.Height ).Returns( 400 );

			var settings = new WindowSettings
			{
				Width = 1000,
				Height = 1000,
				VirtualScreen = screen.Object
			};

			var window = new Mock<IWindowAdapter>();
			window.SetupSet( w => w.Width = 500 ).Verifiable();
			window.SetupSet( w => w.Height = 400 ).Verifiable();

			// Act
			settings.Apply( window.Object );

			// Assert
			window.VerifySet( w => w.Width = 500, Times.Once() );
			window.VerifySet( w => w.Height = 400, Times.Once() );
		}
	}
}