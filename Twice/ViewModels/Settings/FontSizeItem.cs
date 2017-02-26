using GalaSoft.MvvmLight;

namespace Twice.ViewModels.Settings
{
	internal class FontSizeItem : ObservableObject
	{
		public string DisplayName { get; set; }

		public int Size { get; set; }
	}
}