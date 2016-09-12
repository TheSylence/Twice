using System;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Dialogs
{
	internal class ImageEntry
	{
		public ImageEntry( Uri url, bool animated, string title = null )
		{
			ImageUrl = url;
			Title = title ?? url.AbsoluteUri;
			IsAnimated = animated;
			DisplayUrl = url;
		}

		public ImageEntry( StatusMediaViewModel media )
		{
			ImageUrl = media.Url;
			IsAnimated = media.IsAnimated;
			Title = media.DisplayUrl.AbsoluteUri;
			DisplayUrl = media.DisplayUrl;
		}

		public Uri DisplayUrl { get; set; }
		public Uri ImageUrl { get; set; }
		public bool IsAnimated { get; set; }
		public string Title { get; set; }
	}
}