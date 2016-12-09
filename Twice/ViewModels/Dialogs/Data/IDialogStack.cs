namespace Twice.ViewModels.Dialogs.Data
{
	interface IDialogStack
	{
		void Push( DialogData data );
		bool CanGoBack();
		DialogData Pop();
	}
}