using System;

namespace Twice.ViewModels.Dialogs
{
	internal class ContentChangeEventArgs : EventArgs
	{
		public ContentChangeEventArgs( object newContent )
		{
			NewContent = newContent;
		}

		public object NewContent { get; }
	}
}