using System;

namespace Twice.ViewModels.Wizards
{
	internal class PageTypeAttribute : Attribute
	{
		public PageTypeAttribute( Type pageType )
		{
			PageType = pageType;
		}

		public readonly Type PageType;
	}
}