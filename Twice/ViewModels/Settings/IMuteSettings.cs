using System.Collections.Generic;
using System.Windows.Input;
using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	internal interface IMuteSettings : ISettingsSection
	{
		MuteEntry SelectedEntry { get; set; }
		ICommand AddCommand { get; }
		ICommand EditCommand { get; }
		ICollection<MuteEntry> Entries { get; }
		ICommand RemoveCommand { get; }
		string HelpDocument { get; }
		IMuteEditViewModel EditData { get; }
	}
}