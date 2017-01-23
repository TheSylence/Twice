using System.Windows;

namespace Twice.Views
{
	internal interface IWindowAdapter
	{
		double Height { get; set; }
		double Left { get; set; }
		double Top { get; set; }
		double Width { get; set; }
		WindowState WindowState { get; set; }
	}
}