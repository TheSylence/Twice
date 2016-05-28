using MaterialDesignColors;
using System.Collections.Generic;

namespace Twice.Utilities.Ui
{
	internal interface IColorProvider
	{
		void SetAccentColor( string name );

		void SetDarkTheme( bool useDarkTheme );

		void SetFontSize( double size );

		void SetHashtagColor( string name );

		void SetLinkColor( string name );

		void SetMentionColor( string name );

		void SetPrimaryColor( string name );

		IEnumerable<Swatch> AvailableAccentColors { get; }
		IEnumerable<Swatch> AvailablePrimaryColors { get; }
	}
}