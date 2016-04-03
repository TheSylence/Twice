using System;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnActionDispatcher
	{
		event EventHandler BottomReached;

		event EventHandler HeaderClicked;

		void OnBottomReached();

		void OnHeaderClicked();
	}
}