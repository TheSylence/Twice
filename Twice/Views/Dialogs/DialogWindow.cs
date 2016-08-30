using MahApps.Metro.Controls;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Twice.Behaviors;

namespace Twice.Views.Dialogs
{
	/// <summary>
	/// Base class for all modal dialogs.
	/// </summary>
	/// <remarks>
	/// Dialogs are modal because they work like a user would expect them to. Using non-modal
	/// requires toplevel and this could cause some strange behaviors when switching applications, etc.
	/// </remarks>
	[ExcludeFromCodeCoverage]
	public class DialogWindow : MetroWindow
	{
		protected override void OnActivated( EventArgs e )
		{
			base.OnActivated( e );

			// We want to capture mouse event from outside the window
			Mouse.Capture( this, CaptureMode.SubTree );
		}

		protected override void OnMouseDown( MouseButtonEventArgs e )
		{
			var pos = e.GetPosition( this );
			var mainWindowWidth = Application.Current.MainWindow.ActualWidth;
			var mainWindowHeight = Application.Current.MainWindow.ActualHeight;
			var posRelToMain = e.GetPosition( Application.Current.MainWindow );

			bool insideMainWindow = posRelToMain.X >= 0 && posRelToMain.Y >= 0 && posRelToMain.X <= mainWindowWidth && posRelToMain.Y <= mainWindowHeight;
			bool outsideWindow = pos.X < 0 || pos.Y < 0 || pos.X > ActualWidth || pos.Y > ActualHeight;

			bool close = CloseOnClick && outsideWindow && insideMainWindow;
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

		/// <summary>
		/// Flag indicating whether clicking outside the window will close it.
		/// </summary>
		public bool CloseOnClick
		{
			get { return (bool)GetValue( CloseOnClickProperty ); }
			set { SetValue( CloseOnClickProperty, value ); }
		}

		public static readonly DependencyProperty CloseOnClickProperty =
			DependencyProperty.Register( "CloseOnClick", typeof( bool ), typeof( DialogWindow ), new PropertyMetadata( true ) );
	}
}