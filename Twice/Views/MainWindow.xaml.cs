using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using System.Windows.Interop;
using Ninject;
using Twice.Utilities.Os;
using Twice.ViewModels;

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

			App.ApplyWindowSettings( this );

			AttachDebugKeyBindings();
		}

		protected override void OnClosing( CancelEventArgs e )
		{
			base.OnClosing( e );

			App.SaveWindowSettings( this );
		}

		[Conditional( "DEBUG" )]
		private void AttachDebugKeyBindings()
		{
			PreviewKeyDown += MainWindow_PreviewKeyDown;
		}

		private void MainWindow_PreviewKeyDown( object sender, KeyEventArgs e )
		{
			if( Keyboard.IsKeyDown( Key.R ) && Keyboard.IsKeyDown( Key.LeftCtrl ) )

			{
				var kernel = App.Kernel;
				kernel.Get<INotifier>().DisplayMessage( "DEBUG MESSAGE", NotificationType.Information );
			}
		}

		private IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
		{
			if( msg == SingleInstance.WM_SHOWFIRSTINSTANCE )
			{
				Activate();
				handled = true;
			}

			return IntPtr.Zero;
		}
	}
}