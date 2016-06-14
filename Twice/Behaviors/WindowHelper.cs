using System;
using System.Windows;

namespace Twice.Behaviors
{
	internal static class WindowHelper
	{
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