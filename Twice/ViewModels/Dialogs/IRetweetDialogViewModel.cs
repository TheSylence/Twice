using System.Collections.Generic;
using System.Windows.Input;
using Twice.ViewModels.Main;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Dialogs
{
	internal interface IRetweetDialogViewModel : IDialogViewModel, ILoadCallback
	{
		ICollection<AccountEntry> Accounts { get; }
		bool ConfirmationRequired { get; }
		bool ConfirmationSet { get; set; }
		ICommand QuoteCommand { get; }
		ICommand RetweetCommand { get; }
		StatusViewModel Status { get; set; }
	}
}