using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using Twice.Behaviors;

namespace Twice.Views.Dialogs
{
	public class DialogWindow : MetroWindow
	{
		protected override void OnActivated( EventArgs e )
		{
			base.OnActivated( e );

			Mouse.Capture( this, CaptureMode.SubTree );
		}

		protected override void OnMouseDown( MouseButtonEventArgs e )
		{
			var pos = e.GetPosition( this );

			bool close = CloseOnClick && ( pos.X < 0 || pos.Y < 0 || pos.X > ActualWidth || pos.Y > ActualHeight );
			if( close )
			{
				WindowHelper.SetResult( this, false );
				Close();
				e.Handled = true;
			}
			else
			{
				base.OnMouseDown( e );
			}
		}

		public bool CloseOnClick
		{
			get { return (bool)GetValue( CloseOnClickProperty ); }
			set { SetValue( CloseOnClickProperty, value ); }
		}

		public static readonly DependencyProperty CloseOnClickProperty =
			DependencyProperty.Register( "CloseOnClick", typeof( bool ), typeof( DialogWindow ), new PropertyMetadata( true ) );
	}
}