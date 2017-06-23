using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Application = System.Windows.Application;

namespace Twice.Behaviors
{
	/// <summary>
	///     Helper methods for working with WPF Windows
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal static class WindowHelper
	{
		/// <summary>
		///     Centers the specified window at its parent or, if it has no parent, at the screen.
		/// </summary>
		/// <param name="window"> The window to center </param>
		public static void Center( Window window )
		{
			if( window == null )
			{
				return;
			}
			var width = window.ActualWidth;
			var height = window.ActualHeight;

			if( Math.Abs( width ) < 0.1 || Math.Abs( height ) < 0.1 )
			{
				window.SizeChanged += WindowOnSizeChanged;
			}

			CenterInternal( window );
		}

		/// <summary>
		///     Tries to set the <see cref="Window.DialogResult" /> of a window and fails silently.
		/// </summary>
		/// <param name="window"> The window to set the result for </param>
		/// <param name="result"> The result to set </param>
		public static void SetResult( Window window, bool result )
		{
			try
			{
				window.DialogResult = result;
			}
			catch( InvalidOperationException )
			{
			}
		}

		private static void CenterInternal( Window window )
		{
			var width = window.ActualWidth;
			var height = window.ActualHeight;
			var mainWindow = Application.Current.MainWindow;
			var actualPos = GetActualWindowPosition( mainWindow );

			var parentWidth = mainWindow.ActualWidth;
			var parentHeight = mainWindow.ActualHeight;
			var parentLeft = actualPos.X;
			var parentTop = actualPos.Y;

			window.Top = parentTop + ( parentHeight / 2 - height / 2 );
			window.Left = parentLeft + ( parentWidth / 2 - width / 2 );
		}

		/// <summary>
		///     Returns the ACTUAL position of a window, even if it is maximized
		/// </summary>
		/// <param name="window"></param>
		/// <returns></returns>
		private static Point GetActualWindowPosition( Window window )
		{
			if( window.WindowState != WindowState.Maximized )
			{
				return new Point( window.Left, window.Top );
			}

			var windowScreen = Screen.FromHandle( new WindowInteropHelper( window ).Handle );
			return new Point( windowScreen.Bounds.Left, windowScreen.Bounds.Top );
		}

		private static void WindowOnSizeChanged( object sender, SizeChangedEventArgs sizeChangedEventArgs )
		{
			var window = sender as Window;
			Debug.Assert( window != null, "window != null" );
			window.SizeChanged -= WindowOnSizeChanged;

			CenterInternal( window );
		}
	}
}