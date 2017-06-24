using System;
using System.Windows;

namespace Twice.Behaviors
{
	internal class CenterAfterRender : BehaviorBase<Window>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.LayoutUpdated += AssociatedObject_EventHandler;
			AssociatedObject.LocationChanged += AssociatedObject_LocationChanged;
		}

		protected override void OnCleanup()
		{
			AssociatedObject.LayoutUpdated -= AssociatedObject_EventHandler;
		}

		private void AssociatedObject_EventHandler( object sender, EventArgs e )
		{
			if( ShouldCenter )
			{
				WindowHelper.Center( AssociatedObject );
			}
		}

		private void AssociatedObject_LocationChanged( object sender, EventArgs e )
		{
			ShouldCenter = false;
		}

		private bool ShouldCenter = true;
	}
}