using System;

namespace Twice.ViewModels
{
	internal interface IViewController
	{
		/// <summary>
		///     Call this to center view
		/// </summary>
		void Center();

		/// <summary>
		///     Raised when the view should be centered
		/// </summary>
		event EventHandler CenterRequested;

		/// <summary>
		///     Raised when the view should be closed
		/// </summary>
		event EventHandler<CloseEventArgs> CloseRequested;
	}
}