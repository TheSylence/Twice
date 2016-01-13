using System.Collections.Generic;
using System.Globalization;

namespace Twice.ViewModels.Settings
{
	internal interface IVisualSettings
	{
		ICollection<ColorItem> AvailableColors { get; }
		ICollection<FontSizeItem> AvailableFontSizes { get; }
		ICollection<CultureInfo> AvailableLanguages { get; }
		ICollection<ColorItem> AvailableThemes { get; }
		bool InlineMedias { get; set; }
		ColorItem SelectedColor { get; set; }
		FontSizeItem SelectedFontSize { get; set; }
		ColorItem SelectedHashtagColor { get; set; }
		ColorItem SelectedLinkColor { get; set; }
		ColorItem SelectedMentionColor { get; set; }
		ColorItem SelectedTheme { get; set; }
		bool ShowStatusSeparator { get; set; }
	}
}