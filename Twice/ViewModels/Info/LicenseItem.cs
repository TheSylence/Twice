namespace Twice.ViewModels.Info
{
	internal class LicenseItem
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