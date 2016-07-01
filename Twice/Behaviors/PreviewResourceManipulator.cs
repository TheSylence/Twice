using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using Ninject;
using Twice.Utilities.Ui;
using Twice.Views;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class PreviewResourceManipulator : Behavior<StatusContainer>
	{
		public PreviewResourceManipulator()
		{
			ColorProvider = App.Kernel.Get<IColorProvider>();
		}

		private static void OnAccentColorChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var manipulator = d as PreviewResourceManipulator;
			manipulator?.OnAccentColorChanged( (string)e.NewValue );
		}

		private static void OnDarkThemeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var manipulator = d as PreviewResourceManipulator;
			manipulator?.OnDarkThemeChanged( (bool)e.NewValue );
		}

		private static void OnFontSizeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var manipulator = d as PreviewResourceManipulator;
			manipulator?.OnFontSizeChanged( (double)e.NewValue );
		}

		private static void OnHashtagColorChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var manipulator = d as PreviewResourceManipulator;
			manipulator?.OnHashtagColorChanged( (Brush)e.NewValue );
		}

		private static void OnLinkColorChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var manipulator = d as PreviewResourceManipulator;
			manipulator?.OnLinkColorChanged( (Brush)e.NewValue );
		}

		private static void OnMentionColorChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var manipulator = d as PreviewResourceManipulator;
			manipulator?.OnMentionColorChanged( (Brush)e.NewValue );
		}

		private static void OnPrimaryColorChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var manipulator = d as PreviewResourceManipulator;
			manipulator?.OnPrimaryColorChanged( (string)e.NewValue );
		}

		private void OnAccentColorChanged( string newValue )
		{
			if( Dictionary == null )
			{
				return;
			}

			var swatch = ColorProvider.AvailablePrimaryColors.FirstOrDefault( s => s.Name == newValue );
			if( swatch == null )
			{
				return;
			}

			Dictionary["SecondaryAccentBrush"] = new SolidColorBrush( swatch.AccentHues.First( h => h.Name == "Accent700" ).Color );
		}

		private void OnDarkThemeChanged( bool newValue )
		{
			if( Dictionary == null )
			{
				return;
			}

			var resDictionaryName =
				$"pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.{( newValue ? "Dark" : "Light" )}.xaml";
			var dict = new ResourceDictionary {Source = new Uri( resDictionaryName )};

			var keys = new[]
			{
				"PrimaryHueLightBrush", "PrimaryHueLightForegroundBrush",
				"MaterialDesignBackground", "MaterialDesignPaper",
				"MaterialDesignBody", "MaterialDesignBodyLight",
				"PrimaryHueDarkForegroundBrush", "PrimaryHueDarkBrush"
			};
			foreach( var key in keys )
			{
				Dictionary[key] = dict[key];
			}
		}

		private void OnFontSizeChanged( double newValue )
		{
			if( Dictionary == null )
			{
				return;
			}

			Dictionary["GlobalFontSize"] = newValue;
		}

		private void OnHashtagColorChanged( Brush newValue )
		{
			if( Dictionary == null )
			{
				return;
			}

			Dictionary["HashtagBrush"] = newValue;
		}

		private void OnLinkColorChanged( Brush newValue )
		{
			if( Dictionary == null )
			{
				return;
			}

			Dictionary["LinkBrush"] = newValue;
		}

		private void OnMentionColorChanged( Brush newValue )
		{
			if( Dictionary == null )
			{
				return;
			}

			Dictionary["MentionBrush"] = newValue;
		}

		private void OnPrimaryColorChanged( string newValue )
		{
			if( Dictionary == null )
			{
				return;
			}

			var swatch = ColorProvider.AvailablePrimaryColors.FirstOrDefault( s => s.Name == newValue );
			if( swatch == null )
			{
				return;
			}

			// TODO: Update Resouce Dictionaries
		}

		public static readonly DependencyProperty AccentColorNameProperty =
			DependencyProperty.Register( "AccentColorName", typeof(string), typeof(PreviewResourceManipulator),
				new PropertyMetadata( null, OnAccentColorChanged ) );

		public static readonly DependencyProperty DarkThemeProperty =
			DependencyProperty.Register( "DarkTheme", typeof(bool), typeof(PreviewResourceManipulator),
				new PropertyMetadata( true, OnDarkThemeChanged ) );

		public static readonly DependencyProperty DictionaryProperty =
			DependencyProperty.Register( "Dictionary", typeof(ResourceDictionary), typeof(PreviewResourceManipulator),
				new PropertyMetadata( null ) );

		public static readonly DependencyProperty FontSizeProperty =
			DependencyProperty.Register( "FontSize", typeof(double), typeof(PreviewResourceManipulator),
				new PropertyMetadata( 0.0, OnFontSizeChanged ) );

		public static readonly DependencyProperty HashtagColorProperty =
			DependencyProperty.Register( "HashtagColor", typeof(Brush), typeof(PreviewResourceManipulator),
				new PropertyMetadata( null, OnHashtagColorChanged ) );

		public static readonly DependencyProperty LinkColorProperty =
			DependencyProperty.Register( "LinkColor", typeof(Brush), typeof(PreviewResourceManipulator),
				new PropertyMetadata( null, OnLinkColorChanged ) );

		public static readonly DependencyProperty MentionColorProperty =
			DependencyProperty.Register( "MentionColor", typeof(Brush), typeof(PreviewResourceManipulator),
				new PropertyMetadata( null, OnMentionColorChanged ) );

		public static readonly DependencyProperty PrimaryColorNameProperty =
			DependencyProperty.Register( "PrimaryColorName", typeof(string), typeof(PreviewResourceManipulator),
				new PropertyMetadata( null, OnPrimaryColorChanged ) );

		public string AccentColorName
		{
			get { return (string)GetValue( AccentColorNameProperty ); }
			set { SetValue( AccentColorNameProperty, value ); }
		}

		public bool DarkTheme
		{
			get { return (bool)GetValue( DarkThemeProperty ); }
			set { SetValue( DarkThemeProperty, value ); }
		}

		public ResourceDictionary Dictionary
		{
			get { return (ResourceDictionary)GetValue( DictionaryProperty ); }
			set { SetValue( DictionaryProperty, value ); }
		}

		public double FontSize
		{
			get { return (double)GetValue( FontSizeProperty ); }
			set { SetValue( FontSizeProperty, value ); }
		}

		public Brush HashtagColor
		{
			get { return (Brush)GetValue( HashtagColorProperty ); }
			set { SetValue( HashtagColorProperty, value ); }
		}

		public Brush LinkColor
		{
			get { return (Brush)GetValue( LinkColorProperty ); }
			set { SetValue( LinkColorProperty, value ); }
		}

		public Brush MentionColor
		{
			get { return (Brush)GetValue( MentionColorProperty ); }
			set { SetValue( MentionColorProperty, value ); }
		}

		public string PrimaryColorName
		{
			get { return (string)GetValue( PrimaryColorNameProperty ); }
			set { SetValue( PrimaryColorNameProperty, value ); }
		}

		private readonly IColorProvider ColorProvider;
	}
}