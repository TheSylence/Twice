using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
	internal class ScreenRepository : IScreenRepository
	{
		public IVirtualScreen GetScreenFromPosition( double x, double y )
		{
			var allScreens = Screen.AllScreens;

			foreach( var screen in allScreens )
			{
				if( screen.Bounds.Contains( (int)x, (int)y ) )
				{
					return ConstructScreen( screen );
				}
			}

			return ConstructScreen( allScreens.First() );
		}

		private IVirtualScreen ConstructScreen( Screen screen )
		{
			return new VirtualScreen( screen.Bounds.Left, screen.Bounds.Top, screen.Bounds.Width, screen.Bounds.Height );
		}
	}
}