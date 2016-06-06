using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
	class VirtualScreenWrapper : IVirtualScreen
	{
		public double Width => SystemParameters.VirtualScreenWidth;
		public double Height => SystemParameters.VirtualScreenHeight;
	}
}