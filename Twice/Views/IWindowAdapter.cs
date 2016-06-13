using System.Windows;

namespace Twice.Views
{
	interface IWindowAdapter
	{
		double Width { get; set; }
		double Height { get; set; }
		double Top { get; set; }
		double Left { get; set; }
		WindowState WindowState { get; set; }
	}
}