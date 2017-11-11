using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Twice.Views
{
	[ExcludeFromCodeCoverage]
	public class StatusContainer : ContentControl
	{
		public static readonly DependencyProperty DisplayIndicatorsProperty =
			DependencyProperty.Register( "DisplayIndicators", typeof( bool ), typeof( StatusContainer ), new PropertyMetadata( true ) );

		public static readonly DependencyProperty DisplaySourceProperty =
			DependencyProperty.Register( "DisplaySource", typeof( bool ), typeof( StatusContainer ), new PropertyMetadata( false ) );

		public static readonly DependencyProperty EnableHoverIconsProperty =
			DependencyProperty.Register( "EnableHoverIcons", typeof( bool ), typeof( StatusContainer ), new PropertyMetadata( true ) );

		public static readonly DependencyProperty HightlightStatusProperty =
			DependencyProperty.Register( "HightlightStatus", typeof( bool ), typeof( StatusContainer ), new PropertyMetadata( false ) );

		public static readonly DependencyProperty ShowFavoritesProperty =
			DependencyProperty.Register( "ShowFavorites", typeof( bool ), typeof( StatusContainer ), new PropertyMetadata( false ) );

		public static readonly DependencyProperty ShowRetweetsProperty =
			DependencyProperty.Register( "ShowRetweets", typeof( bool ), typeof( StatusContainer ), new PropertyMetadata( false ) );

		public bool DisplayIndicators
		{
			get => (bool)GetValue( DisplayIndicatorsProperty );
			set => SetValue( DisplayIndicatorsProperty, value );
		}

		public bool DisplaySource
		{
			get => (bool)GetValue( DisplaySourceProperty );
			set => SetValue( DisplaySourceProperty, value );
		}

		public bool EnableHoverIcons
		{
			get => (bool)GetValue( EnableHoverIconsProperty );
			set => SetValue( EnableHoverIconsProperty, value );
		}

		public bool HightlightStatus
		{
			get => (bool)GetValue( HightlightStatusProperty );
			set => SetValue( HightlightStatusProperty, value );
		}

		public bool ShowFavorites
		{
			get => (bool)GetValue( ShowFavoritesProperty );
			set => SetValue( ShowFavoritesProperty, value );
		}

		public bool ShowRetweets
		{
			get => (bool)GetValue( ShowRetweetsProperty );
			set => SetValue( ShowRetweetsProperty, value );
		}
	}
}