using System;
using System.Windows;

namespace Twice.Behaviors
{
	/// <summary>
	/// Helper methods for working with WPF Windows
	/// </summary>
	internal static class WindowHelper
	{
		/// <summary>
		/// Tries to set the <see cref="Window.DialogResult"/> of a window and fails silently.
		/// </summary>
		/// <param name="window">The window to set the result for</param>
		/// <param name="result">The result to set</param>
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