using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;

// ReSharper disable InconsistentNaming

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class StaticWindow : Behavior<Window>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;
		}

		private void AssociatedObject_SourceInitialized( object sender, EventArgs e )
		{
			WindowInteropHelper helper = new WindowInteropHelper( AssociatedObject );
			HwndSource source = HwndSource.FromHwnd( helper.Handle );
			Debug.Assert( source != null, "source != null" );
			source.AddHook( WndProc );
		}

		private IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
		{
			switch( msg )
			{
			case WM_SYSCOMMAND:
				int command = wParam.ToInt32() & 0xfff0;
				if( command == SC_MOVE )
				{
					handled = true;
				}
				break;
			}
			return IntPtr.Zero;
		}

		private const int SC_MOVE = 0xF010;
		private const int WM_SYSCOMMAND = 0x0112;
	}
}