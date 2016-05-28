using MaterialDesignColors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using Twice.Models.Configuration;
using Twice.Utilities.Ui;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass, ExcludeFromCodeCoverage]
	public class VisualSettingsTests
	{
		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void PropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Visual ).Returns( new VisualConfig() );

			var colors = new Mock<IColorProvider>();
			var lang = new Mock<ILanguageProvider>();

			var vm = new VisualSettings( cfg.Object, colors.Object, lang.Object );
			var tester = new PropertyChangedTester( vm );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SavedValuesAreAppliedDuringConstruction()
		{
			// Arrange
			var visual = new VisualConfig
			{
				AccentColor = "blue",
				FontSize = 13,
				HashtagColor = "red",
				InlineMedia = true,
				LinkColor = "green",
				MentionColor = "red",
				PrimaryColor = "yellow",
				UseDarkTheme = true
			};

			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Visual ).Returns( visual );

			var colors = new Mock<IColorProvider>();
			var colorList = new[]
			{CreateSwatch( "blue" ), CreateSwatch( "red" ), CreateSwatch( "green" ), CreateSwatch( "yellow" )};
			colors.SetupGet( c => c.AvailableAccentColors ).Returns( colorList );
			colors.SetupGet( c => c.AvailablePrimaryColors ).Returns( colorList );

			var lng = new Mock<ILanguageProvider>();
			lng.SetupGet( l => l.CurrentCulture ).Returns( CultureInfo.CreateSpecificCulture( "en-US" ) );

			// Act
			var vm = new VisualSettings( cfg.Object, colors.Object, lng.Object );

			// Assert
			Assert.AreEqual( visual.AccentColor, vm.SelectedAccentColor.Name );
			Assert.AreEqual( visual.FontSize, vm.SelectedFontSize.Size );
			Assert.AreEqual( visual.HashtagColor, vm.SelectedHashtagColor.Name );
			Assert.AreEqual( visual.InlineMedia, vm.InlineMedias );
			Assert.AreEqual( visual.LinkColor, vm.SelectedLinkColor.Name );
			Assert.AreEqual( visual.MentionColor, vm.SelectedMentionColor.Name );
			Assert.AreEqual( visual.PrimaryColor, vm.SelectedPrimaryColor.Name );
			Assert.AreEqual( visual.UseDarkTheme, vm.UseDarkTheme );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SettingsAreCorrectlySaved()
		{
			// Arrange
			var colorList = new[]
			{CreateSwatch( "blue" ), CreateSwatch( "red" ), CreateSwatch( "green" ), CreateSwatch( "yellow" )};

			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.Visual ).Returns( new VisualConfig() );

			var colors = new Mock<IColorProvider>();
			colors.SetupGet( c => c.AvailableAccentColors ).Returns( colorList );
			colors.SetupGet( c => c.AvailablePrimaryColors ).Returns( colorList );

			colors.Setup( c => c.SetDarkTheme( true ) ).Verifiable();
			colors.Setup( c => c.SetFontSize( 16 ) ).Verifiable();
			colors.Setup( c => c.SetHashtagColor( "yellow" ) ).Verifiable();
			colors.Setup( c => c.SetLinkColor( "red" ) ).Verifiable();
			colors.Setup( c => c.SetMentionColor( "blue" ) ).Verifiable();
			colors.Setup( c => c.SetAccentColor( "green" ) ).Verifiable();
			colors.Setup( c => c.SetPrimaryColor( "blue" ) ).Verifiable();

			var lang = new Mock<ILanguageProvider>();

			var vm = new VisualSettings( cfg.Object, colors.Object, lang.Object );
			vm.SelectedAccentColor = vm.AvailableAccentColors.FirstOrDefault( c => c.Name == "green" );
			vm.SelectedPrimaryColor = vm.AvailableAccentColors.FirstOrDefault( c => c.Name == "blue" );
			vm.SelectedHashtagColor = vm.AvailableAccentColors.FirstOrDefault( c => c.Name == "yellow" );
			vm.SelectedLinkColor = vm.AvailableAccentColors.FirstOrDefault( c => c.Name == "red" );
			vm.SelectedMentionColor = vm.AvailableAccentColors.FirstOrDefault( c => c.Name == "blue" );
			vm.InlineMedias = true;
			vm.SelectedFontSize = vm.AvailableFontSizes.FirstOrDefault( f => f.Size == 16 );
			vm.UseDarkTheme = true;

			var saved = new Mock<IConfig>();
			var visual = new VisualConfig();
			saved.SetupGet( s => s.Visual ).Returns( visual );

			// Act
			vm.SaveTo( saved.Object );

			// Assert
			Assert.AreEqual( vm.SelectedAccentColor.Name, visual.AccentColor );
			Assert.AreEqual( vm.SelectedFontSize.Size, visual.FontSize );
			Assert.AreEqual( vm.SelectedHashtagColor.Name, visual.HashtagColor );
			Assert.AreEqual( vm.SelectedLinkColor.Name, visual.LinkColor );
			Assert.AreEqual( vm.SelectedMentionColor.Name, visual.MentionColor );
			Assert.AreEqual( vm.SelectedPrimaryColor.Name, visual.PrimaryColor );
			Assert.AreEqual( vm.InlineMedias, visual.InlineMedia );
			Assert.AreEqual( vm.UseDarkTheme, visual.UseDarkTheme );

			colors.Verify( c => c.SetDarkTheme( true ), Times.Once() );
			colors.Verify( c => c.SetFontSize( 16 ), Times.Once() );
			colors.Verify( c => c.SetHashtagColor( "yellow" ), Times.Once() );
			colors.Verify( c => c.SetLinkColor( "red" ), Times.Once() );
			colors.Verify( c => c.SetMentionColor( "blue" ), Times.Once() );
			colors.Verify( c => c.SetAccentColor( "green" ), Times.Once() );
			colors.Verify( c => c.SetPrimaryColor( "blue" ), Times.Once() );
		}

		private static Swatch CreateSwatch( string name )
		{
			var primaryHues = new[] { new Hue( "p", Colors.Red, Colors.Blue ) };
			var accentHues = new[] { new Hue( "a", Colors.Green, Colors.Yellow ) };

			return new Swatch( name, primaryHues, accentHues );
		}
	}
}