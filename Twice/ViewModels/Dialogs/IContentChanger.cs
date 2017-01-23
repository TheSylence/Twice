using System;

namespace Twice.ViewModels.Dialogs
{
	internal interface IContentChanger
	{
		event EventHandler<ContentChangeEventArgs> ContentChange;

		void ChangeContent( object newContent );
	}
}