using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Twice.ViewModels;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class ScrollToEnd : BehaviorBase<ItemsControl>
	{
		private static void OnControllerChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var b = d as ScrollToEnd;
			b?.OnControllerChanged( (IScrollController)e.NewValue, (IScrollController)e.OldValue );
		}

		private void Controller_ScrollRequested( object sender, EventArgs e )
		{
			int index = ScrollBottom
				? AssociatedObject.ItemContainerGenerator.Items.Count - 1
				: 0;

			var element = AssociatedObject.ItemContainerGenerator.ContainerFromIndex( index ) as FrameworkElement;
			element?.BringIntoView();
		}

		private void OnControllerChanged( IScrollController newController, IScrollController oldController )
		{
			if( oldController != null )
			{
				oldController.ScrollRequested -= Controller_ScrollRequested;
			}

			if( newController != null )
			{
				newController.ScrollRequested += Controller_ScrollRequested;
			}
		}

		public static readonly DependencyProperty ControllerProperty =
			DependencyProperty.Register( "Controller", typeof(IScrollController), typeof(ScrollToEnd),
				new PropertyMetadata( null, OnControllerChanged ) );

		public static readonly DependencyProperty ScrollBottomProperty =
			DependencyProperty.Register( "ScrollBottom", typeof(bool), typeof(ScrollToEnd), new PropertyMetadata( true ) );

		public IScrollController Controller
		{
			get { return (IScrollController)GetValue( ControllerProperty ); }
			set { SetValue( ControllerProperty, value ); }
		}

		public bool ScrollBottom
		{
			get { return (bool)GetValue( ScrollBottomProperty ); }
			set { SetValue( ScrollBottomProperty, value ); }
		}
	}
}