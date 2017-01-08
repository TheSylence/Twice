using System.Collections.Generic;

namespace Twice.ViewModels.Dialogs.Data
{
	internal class DialogStack : IDialogStack
	{
		public bool CanGoBack()
		{
			return Data.Count > 1;
		}

		public DialogData Pop()
		{
			return Data.Pop();
		}

		public void Push( DialogData data )
		{
			Data.Push( data );
		}

		public TResult ResultSetup<TViewModel, TResult>( TViewModel vm ) where TViewModel : class
		{
			var topData = Data.Peek();
			return (TResult)topData.GetResult( vm );
		}

		public void Setup<TViewModel>( TViewModel vm ) where TViewModel : class
		{
			var topData = Data.Peek();
			topData.Setup( vm );
		}

		private readonly Stack<DialogData> Data = new Stack<DialogData>();
	}
}