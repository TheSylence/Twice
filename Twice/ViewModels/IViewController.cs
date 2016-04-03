using System;

namespace Twice.ViewModels
{
	internal interface IViewController
	{
		event EventHandler<CloseEventArgs> CloseRequested;
	}
}