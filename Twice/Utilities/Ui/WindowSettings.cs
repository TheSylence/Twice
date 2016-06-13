using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Twice.Views;

namespace Twice.Utilities.Ui
{
	internal class WindowSettings
	{
		public static WindowSettings Load( string fileName )
		{
			if( !File.Exists( fileName ) )
			{
				return null;
			}

			var json = File.ReadAllText( fileName );
			return JsonConvert.DeserializeObject<WindowSettings>( json );
		}

		/// <summary>
		///     Applies the saved settings to a window.
		/// </summary>
		/// <param name="window">The window</param>
		public void Apply( IWindowAdapter window )
		{
			Point size = EnsureWindowSize();
			Point pos = EnsureWindowVisibility( size );

			window.Width = size.X;
			window.Height = size.Y;
			window.Left = pos.X;
			window.Top = pos.Y;
			window.WindowState = State;
		}

		/// <summary>
		///     Saves the current state of a window to a file.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>
		///     <c>true</c> if save was successful; otherwise (when window is minimized) <c>false</c>
		/// </returns>
		public bool Save( IWindowAdapter window )
		{
			return Save( Constants.IO.WindowSettingsFileName, window );
		}

		internal bool Save( string fileName, IWindowAdapter window )
		{
			if( window.WindowState == WindowState.Minimized )
			{
				return false;
			}

			Width = window.Width;
			Height = window.Height;
			Top = window.Top;
			Left = window.Left;
			State = window.WindowState;

			var json = JsonConvert.SerializeObject( this );
			File.WriteAllText( fileName, json );
			return true;
		}

		private Point EnsureWindowSize()
		{
			var x = Width;
			var y = Height;

			// Make sure the window is at most as large as the available screen space.

			if( x > VirtualScreen.Width )
			{
				x = VirtualScreen.Width;
			}
			if( y > VirtualScreen.Height )
			{
				y = VirtualScreen.Height;
			}

			return new Point( x, y );
		}

		private Point EnsureWindowVisibility( Point size )
		{
			var x = Left;
			var y = Top;

			// If the window is half off the screen, move it into view
			if( y + size.Y / 2 > VirtualScreen.Height )
			{
				y = VirtualScreen.Height - size.Y;
			}
			if( x + size.X / 2 > VirtualScreen.Width )
			{
				x = VirtualScreen.Width - size.X;
			}

			if( y < 0 )
			{
				y = 0;
			}
			if( x < 0 )
			{
				x = 0;
			}

			return new Point( x, y );
		}

		public double Height { get; set; }
		public double Left { get; set; }
		public WindowState State { get; set; }
		public double Top { get; set; }
		public double Width { get; set; }
		internal IVirtualScreen VirtualScreen { get; set; } = new VirtualScreenWrapper();
	}
}