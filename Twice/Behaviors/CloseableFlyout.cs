using System.Diagnostics.CodeAnalysis;
using System.Windows;
using MahApps.Metro.Controls;
using Twice.ViewModels;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class CloseableFlyout : BehaviorBase<Flyout>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
			AssociatedObject_DataContextChanged( null, new DependencyPropertyChangedEventArgs() );
		}

		private void AssociatedObject_DataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
		{
			if( AttachedController != null )
			{
				AttachedController.CloseRequested -= Controller_CloseRequested;
			}

			AttachedController = AssociatedObject.DataContext as IViewController;
			if( AttachedController != null )
			{
				AttachedController.CloseRequested += Controller_CloseRequested;
			}
		}

		private void Controller_CloseRequested( object sender, CloseEventArgs e )
		{
			AssociatedObject.IsOpen = false;
		}

		private IViewController AttachedController;
	}
}