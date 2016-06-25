using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using Twice.Views;

namespace Twice.Behaviors
{
	internal class PreviewResourceManipulator : Behavior<StatusContainer>
	{
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
	}
}