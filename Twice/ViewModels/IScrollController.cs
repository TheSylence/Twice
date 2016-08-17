using System;

namespace Twice.ViewModels
{
	public interface IScrollController
	{
		event EventHandler ScrollRequested;
	}
}