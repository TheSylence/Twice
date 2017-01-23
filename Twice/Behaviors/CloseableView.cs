using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Twice.ViewModels;

namespace Twice.Behaviors
{
	/// <summary>
	///  Allows a window to be closed from the window's DataContext 
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal class CloseableView : BehaviorBase<Window>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;

			CurrentController = AssociatedObject.DataContext as IViewController;
			if( CurrentController != null )
			{
				CurrentController.CloseRequested += Controller_CloseRequested;
				CurrentController.CenterRequested += Controller_CenterRequested;
			}

			AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
		}

		private void AssociatedObject_DataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
		{
			if( CurrentController != null )
			{
				CurrentController.CloseRequested -= Controller_CloseRequested;
				CurrentController.CenterRequested -= Controller_CenterRequested;
			}

			CurrentController = AssociatedObject.DataContext as IViewController;
			if( CurrentController != null )
			{
				CurrentController.CloseRequested += Controller_CloseRequested;
				CurrentController.CenterRequested += Controller_CenterRequested;
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

		private void Controller_CenterRequested( object sender, System.EventArgs e )
		{
			WindowHelper.Center( AssociatedObject );
		}

		private void Controller_CloseRequested( object sender, CloseEventArgs e )
		{
			WindowHelper.SetResult( AssociatedObject, e.Result );
			CloseWindow();
		}

		private IViewController CurrentController;
	}
}