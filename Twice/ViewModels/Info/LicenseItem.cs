using GalaSoft.MvvmLight;

namespace Twice.ViewModels.Info
{
	internal class LicenseItem : ObservableObject
	{
		public LicenseItem( string name, string content )
		{
			Name = name;
			Content = content;
		}

		public string Content { get; }
		public string Name { get; }
	}
}