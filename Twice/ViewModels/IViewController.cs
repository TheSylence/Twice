using System;

namespace Twice.ViewModels
{
	internal interface IViewController
	{
		/// <summary>
		/// Raised when the view should be closed
		/// </summary>
		event EventHandler<CloseEventArgs> CloseRequested;

		/// <summary>
		/// Raised when the view should be centered
		/// </summary>
		event EventHandler CenterRequested;
	}
}