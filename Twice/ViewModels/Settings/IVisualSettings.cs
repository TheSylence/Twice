using System.Collections.Generic;

namespace Twice.ViewModels.Settings
{
	internal interface IVisualSettings : ISettingsSection
	{
		ICollection<ColorItem> AvailableAccentColors { get; }
		ICollection<ColorItem> AvailablePrimaryColors { get; }
		ICollection<FontSizeItem> AvailableFontSizes { get; }
		bool InlineMedias { get; set; }
		ColorItem SelectedAccentColor { get; set; }
		ColorItem SelectedPrimaryColor { get; set; }
		FontSizeItem SelectedFontSize { get; set; }
		ColorItem SelectedHashtagColor { get; set; }
		ColorItem SelectedLinkColor { get; set; }
		ColorItem SelectedMentionColor { get; set; }
		bool UseDarkTheme { get; set; }
	}
}