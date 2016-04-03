using System;
using System.Windows.Input;

namespace Twice.ViewModels.Settings
{
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
}