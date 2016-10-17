using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
	internal class ColorProvider : IColorProvider
	{
		public ColorProvider()
		{
			Swatches = new SwatchesProvider();
			Palette = new PaletteHelper();
		}

		public void SetAccentColor( string name )
		{
			Palette.ReplaceAccentColor( name );
		}

		public void SetDarkTheme( bool useDarkTheme )
		{
			Palette.SetLightDark( useDarkTheme );
		}

		public void SetFontSize( double size )
		{
			Application.Current.Resources["GlobalFontSize"] = size;
		}

		public void SetHashtagColor( string name )
		{
			Application.Current.Resources["HashtagBrush"] = new SolidColorBrush( GetColorByName( name ) );
		}

		public void SetLinkColor( string name )
		{
			Application.Current.Resources["LinkBrush"] = new SolidColorBrush( GetColorByName( name ) );
		}

		public void SetMentionColor( string name )
		{
			Application.Current.Resources["MentionBrush"] = new SolidColorBrush( GetColorByName( name ) );
		}

		public void SetPrimaryColor( string name )
		{
			Palette.ReplacePrimaryColor( name );
		}

		private Color GetColorByName( string name )
		{
			return Swatches.Swatches.First( s => s.Name == name ).ExemplarHue.Color;
		}

		public IEnumerable<Swatch> AvailableAccentColors => Swatches.Swatches.Where( a => a.IsAccented );
		public IEnumerable<Swatch> AvailablePrimaryColors => Swatches.Swatches;
		private readonly PaletteHelper Palette;
		private readonly SwatchesProvider Swatches;
	}
}