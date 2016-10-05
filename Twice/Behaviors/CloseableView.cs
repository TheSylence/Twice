using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Twice.ViewModels;

namespace Twice.Behaviors
{
	/// <summary>
	/// Allows a window to be closed from the window's DataContext
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal class CloseableView : BehaviorBase<Window>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;

			var controller = AssociatedObject.DataContext as IViewController;
			if( controller != null )
			{
				controller.CloseRequested += Controller_CloseRequested;
			}
		}

		private void AssociatedObject_PreviewKeyDown( object sender, KeyEventArgs e )
		{
			if( e.Key == Key.Escape )
			{
				WindowHelper.SetResult( AssociatedObject, false );
				CloseWindow();
			}
		}

		private void CloseWindow()
		{
			AssociatedObject.Close();
		}

		private void Controller_CloseRequested( object sender, CloseEventArgs e )
		{
			WindowHelper.SetResult( AssociatedObject, e.Result );
			CloseWindow();
		}
	}
}