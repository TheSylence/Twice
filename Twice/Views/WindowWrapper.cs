using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Twice.Views
{
	[ExcludeFromCodeCoverage]
	internal class WindowWrapper : IWindowAdapter
	{
		public WindowWrapper( Window window )
		{
			Window = window;
		}

		public double Height { get { return Window.Height; } set { Window.Height = value; } }
		public double Left { get { return Window.Left; } set { Window.Left = value; } }
		public double Top { get { return Window.Top; } set { Window.Top = value; } }

		public double Width
		{
			get { return Window.Width; }
			set { Window.Width = value; }
		}

		public WindowState WindowState { get { return Window.WindowState; } set { Window.WindowState = value; } }
		private readonly Window Window;
	}
}