using System;
using System.Windows.Input;

namespace Twice.ViewModels.Settings
{
	internal interface IMuteEditViewModel
	{
		event EventHandler Cancelled;

		event EventHandler<MuteEditArgs> Saved;

		ICommand CancelCommand { get; }
		DateTime EndDate { get; set; }
		string Filter { get; set; }
		bool HasEndDate { get; set; }
		ICommand SaveCommand { get; }
	}
}