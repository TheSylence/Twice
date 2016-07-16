using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Twice.Behaviors
{
	internal class MediaController : Behavior<MediaElement>
	{
		protected override void OnAttached()
		{
			AssociatedObject.MediaEnded += AssociatedObject_MediaEnded;

			base.OnAttached();
		}

		private void AssociatedObject_MediaEnded( object sender, RoutedEventArgs e )
		{
			if( IsAnimated )
			{
				AssociatedObject.Position = TimeSpan.Zero;
				AssociatedObject.Play();
			}

			e.Handled = true;
		}

		private void AssociatedObject_MediaOpened( object sender, RoutedEventArgs e )
		{
			AssociatedObject.Play();
		}

		public static readonly DependencyProperty IsAnimatedProperty =
			DependencyProperty.Register( "IsAnimated", typeof(bool), typeof(MediaController), new PropertyMetadata( false ) );

		public bool IsAnimated
		{
			get { return (bool)GetValue( IsAnimatedProperty ); }
			set { SetValue( IsAnimatedProperty, value ); }
		}
	}
}