using System;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnActionDispatcher
	{
		void OnBottomReached();

		void OnHeaderClicked();
		event EventHandler BottomReached;

		event EventHandler HeaderClicked;
	}
}