using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Twice.Behaviors
{
	/// <summary>
	///  Helper methods for working with WPF Windows 
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal static class WindowHelper
	{
		/// <summary>
		/// Centers the specified window at its parent or, if it has no parent, at the screen.
		/// </summary>
		/// <param name="window">The window to center</param>
		public static void Center( Window window )
		{
			var parentWidth = window.Owner?.ActualWidth ?? SystemParameters.WorkArea.Width;
			var parentHeight = window.Owner?.ActualHeight ?? SystemParameters.WorkArea.Height;
			var parentLeft = window.Owner?.Left ?? 0;
			var parentTop = window.Owner?.Top ?? 0;

			var width = window.ActualWidth;
			var height = window.ActualHeight;

			window.Top = parentTop + ( parentHeight / 2 - height / 2 ) ;
			window.Left = parentLeft + ( parentWidth / 2 - width / 2 ) ;
		}

		/// <summary>
		///  Tries to set the <see cref="Window.DialogResult" /> of a window and fails silently. 
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
	}
}