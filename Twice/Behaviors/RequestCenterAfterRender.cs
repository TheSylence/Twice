using System;
using System.Windows;
using Twice.ViewModels;
using Twice.Views;

namespace Twice.Behaviors
{
	internal class RequestCenterAfterRender : BehaviorBase<FrameworkElement>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.LayoutUpdated += AssociatedObject_EventHandler;
			AssociatedObject.Loaded += ( s, e ) =>
			{
				var window = VisualTreeWalker.FindParent<Window>( AssociatedObject );
				window.LocationChanged += ( ss, ee ) =>
				{
					if( !BlockReentrance )
					{
						ShouldCenter = false;
					}
				};
			};
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.LayoutUpdated -= AssociatedObject_EventHandler;
		}

		private void AssociatedObject_EventHandler( object sender, EventArgs e )
		{
			if( ShouldCenter )
			{
				var viewController = AssociatedObject.DataContext as IViewController;
				BlockReentrance = true;
				viewController?.Center();
				BlockReentrance = false;
			}
		}

		private bool BlockReentrance;
		private bool ShouldCenter = true;
	}
}