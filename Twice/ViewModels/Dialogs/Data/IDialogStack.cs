namespace Twice.ViewModels.Dialogs.Data
{
	internal interface IDialogStack
	{
		bool CanGoBack();

		void Clear();

		bool Push( DialogData data );

		void Remove();

		TResult ResultSetup<TViewModel, TResult>( TViewModel vm ) where TViewModel : class;

		void Setup<TViewModel>( TViewModel vm ) where TViewModel : class;

		void Setup( IContentChanger contextChanger );
	}
}