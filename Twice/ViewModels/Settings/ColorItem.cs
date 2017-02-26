using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Twice.ViewModels.Settings
{
	internal class ColorItem : ObservableObject
	{
		public Brush BorderBrush { get; set; }

		public Brush ColorBrush { get; set; }

		public string Name { get; set; }
	}
}