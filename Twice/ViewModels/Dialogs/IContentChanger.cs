using System;

namespace Twice.ViewModels.Dialogs
{
	internal interface IContentChanger
	{
		void ChangeContent( object newContent );
		event EventHandler<ContentChangeEventArgs> ContentChange;
	}
}