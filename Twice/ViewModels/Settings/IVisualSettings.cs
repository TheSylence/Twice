using System.Collections.Generic;

namespace Twice.ViewModels.Settings
{
	internal interface IVisualSettings : ISettingsSection
	{
		ICollection<ColorItem> AvailableAccentColors { get; }
		ICollection<FontSizeItem> AvailableFontSizes { get; }
		ICollection<ColorItem> AvailablePrimaryColors { get; }
		bool InlineMedias { get; set; }
		ColorItem SelectedAccentColor { get; set; }
		FontSizeItem SelectedFontSize { get; set; }
		ColorItem SelectedHashtagColor { get; set; }
		ColorItem SelectedLinkColor { get; set; }
		ColorItem SelectedMentionColor { get; set; }
		ColorItem SelectedPrimaryColor { get; set; }
		bool UseDarkTheme { get; set; }
	}
}