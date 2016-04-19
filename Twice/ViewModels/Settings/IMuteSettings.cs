using System.Collections.Generic;
using System.Windows.Input;
using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	internal interface IMuteSettings : ISettingsSection
	{
		ICommand AddCommand { get; }
		ICommand EditCommand { get; }
		IMuteEditViewModel EditData { get; }
		ICollection<MuteEntry> Entries { get; }
		string HelpDocument { get; }
		ICommand RemoveCommand { get; }
		MuteEntry SelectedEntry { get; set; }
	}
}