using System;
using System.Collections.Generic;

namespace Twice.ViewModels.Dialogs.Data
{
	class DialogStack : IDialogStack
	{
		public void Push( DialogData data )
		{
			Data.Push( data );
		}

		public bool CanGoBack()
		{
			return Data.Count > 1;
		}

		public DialogData Pop()
		{
			return Data.Pop();
		}

		private readonly Stack<DialogData> Data = new Stack<DialogData>();
	}
}