using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Anotar.NLog;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class MediaController : BehaviorBase<MediaElement>
	{
		protected override void OnAttached()
		{
			AssociatedObject.IsMuted = MuteAudio;
			AssociatedObject.MediaEnded += AssociatedObject_MediaEnded;
			AssociatedObject.MediaFailed += AssociatedObject_MediaFailed;
			AssociatedObject.MediaOpened += AssociatedObject_MediaOpened;

			Stop();
		}

		protected override void OnCleanup()
		{
			Stop();
		}

		private void AssociatedObject_MediaEnded( object sender, RoutedEventArgs e )
		{
			ResetBeforePlay = true;

			if( IsAnimated && Loop )
			{
				AssociatedObject.Position = TimeSpan.Zero;
				Play();
			}
			else
			{
				IsPlaying = false;
			}
		}

		private void AssociatedObject_MediaFailed( object sender, ExceptionRoutedEventArgs e )
		{
			LogTo.WarnException( "Media failed", e.ErrorException );
		}

		private void AssociatedObject_MediaOpened( object sender, RoutedEventArgs e )
		{
			AssociatedObject.IsMuted = MuteAudio;
			Play();
			ResetBeforePlay = false;
		}

		private static void OnIsPlayingChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var ctrl = d as MediaController;
			ctrl?.OnIsPlayingChanged( (bool)e.NewValue );
		}

		private void OnIsPlayingChanged( bool isPlaying )
		{
			if( AssociatedObject == null || !IsAnimated )
			{
				return;
			}

			if( isPlaying )
			{
				if( ResetBeforePlay )
				{
					AssociatedObject.Position = TimeSpan.Zero;
				}

				ResetBeforePlay = false;
				Play();
			}
			else
			{
				Pause();
			}
		}

		private static void OnMuteAudioChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var ctrl = d as MediaController;
			ctrl?.OnMuteAudioChanged( (bool)e.NewValue );
		}

		private void OnMuteAudioChanged( bool mute )
		{
			if( AssociatedObject == null || !IsAnimated )
			{
				return;
			}

			AssociatedObject.IsMuted = mute;
		}

		private void Pause()
		{
			try
			{
				AssociatedObject.Pause();
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to pause", ex );
			}
		}

		private void Play()
		{
			try
			{
				AssociatedObject.Play();
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to play", ex );
			}
		}

		private void Stop()
		{
			try
			{
				AssociatedObject?.Stop();
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to stop.", ex );
			}
		}

		public static readonly DependencyProperty IsAnimatedProperty =
			DependencyProperty.Register( "IsAnimated", typeof( bool ), typeof( MediaController ), new PropertyMetadata( false ) );

		public static readonly DependencyProperty IsPlayingProperty =
			DependencyProperty.Register( "IsPlaying", typeof( bool ), typeof( MediaController ), new PropertyMetadata( true, OnIsPlayingChanged ) );

		public static readonly DependencyProperty LoopProperty = DependencyProperty.Register( "Loop", typeof( bool ), typeof( MediaController ),
			new PropertyMetadata( true ) );

		public static readonly DependencyProperty MuteAudioProperty =
			DependencyProperty.Register( "MuteAudio", typeof( bool ), typeof( MediaController ), new PropertyMetadata( true, OnMuteAudioChanged ) );

		public bool IsAnimated
		{
			get => (bool)GetValue( IsAnimatedProperty );
			set => SetValue( IsAnimatedProperty, value );
		}

		public bool IsPlaying
		{
			get => (bool)GetValue( IsPlayingProperty );
			set => SetValue( IsPlayingProperty, value );
		}

		public bool Loop
		{
			get => (bool)GetValue( LoopProperty );
			set => SetValue( LoopProperty, value );
		}

		public bool MuteAudio
		{
			get => (bool)GetValue( MuteAudioProperty );
			set => SetValue( MuteAudioProperty, value );
		}

		private bool ResetBeforePlay;
	}
}