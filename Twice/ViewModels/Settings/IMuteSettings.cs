using System;
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

	interface IMuteEditViewModel
	{
		event EventHandler<MuteEditArgs> Saved;
		event EventHandler Cancelled;

		string Filter { get; set; }
		DateTime EndDate { get; set; }
		bool HasEndDate { get; set; }
		ICommand SaveCommand { get; }
		ICommand CancelCommand { get; }
	}

	class MuteEditArgs : EventArgs
	{
		public MuteEditArgs( MuteEditAction action, string filter, DateTime? endDate )
		{
			Action = action;
			Filter = filter;
			EndDate = endDate;
		}

		public readonly string Filter;
		public readonly DateTime? EndDate;
		public readonly MuteEditAction Action;
	}

	enum MuteEditAction
	{
		Add,
		Edit
	}
}