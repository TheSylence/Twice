using System;

namespace Twice.ViewModels.Dialogs
{
	internal class ImageEntry
	{
		public ImageEntry( Uri url, string title = null )
		{
			ImageUrl = url;
			Title = title ?? url.AbsoluteUri;
		}

		public Uri ImageUrl { get; set; }
		public string Title { get; set; }
	}
}