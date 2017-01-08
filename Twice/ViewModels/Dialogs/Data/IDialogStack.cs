namespace Twice.ViewModels.Dialogs.Data
{
	internal interface IDialogStack
	{
		bool CanGoBack();

		DialogData Pop();

		void Push( DialogData data );

		TResult ResultSetup<TViewModel, TResult>( TViewModel vm ) where TViewModel : class;

		void Setup<TViewModel>( TViewModel vm ) where TViewModel : class;
	}
}