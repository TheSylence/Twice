using System.Windows;

namespace Twice.Views
{
	class WindowWrapper : IWindowAdapter
	{
		public WindowWrapper( Window window )
		{
			Window = window;
		}

		private readonly Window Window;
		public double Width
		{
			get { return Window.Width; }
			set { Window.Width = value; }
		}
		public double Height { get { return Window.Height; } set { Window.Height = value; } }
		public double Top { get { return Window.Top; } set { Window.Top = value; } }
		public double Left { get { return Window.Left; } set { Window.Left = value; } }
		public WindowState WindowState { get { return Window.WindowState; } set { Window.WindowState = value; } }
	}
}