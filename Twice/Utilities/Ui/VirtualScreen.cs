using System.Diagnostics.CodeAnalysis;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
	internal class VirtualScreen : IVirtualScreen
	{
		public VirtualScreen( double left, double top, double width, double height )
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		public double Height { get; }
		public double Left { get; }
		public double Top { get; }
		public double Width { get; }
	}
}