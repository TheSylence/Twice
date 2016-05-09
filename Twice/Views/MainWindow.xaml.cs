using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Interop;
using Twice.Utilities.Os;

namespace Twice.Views
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			var handle = new WindowInteropHelper( this ).EnsureHandle();
			var source = HwndSource.FromHwnd( handle );
			source?.AddHook( WndProc );
		}

		private IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
		{
			if( msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
			{
				Activate();
				handled = true;
			}

			return IntPtr.Zero;
		}
	}
}