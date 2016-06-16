using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Twice.Behaviors
{
	internal class ScrollIntoView : Behavior<Button>
	{
		protected override void OnAttached()
		{
			AssociatedObject.Click += AssociatedObject_Click;
		}

		private void AssociatedObject_Click( object sender, RoutedEventArgs e )
		{
			if( Item != null && Control != null )
			{
				var element = Control.ItemContainerGenerator.ContainerFromItem( Item ) as FrameworkElement;
				if( element != null )
				{
					element.BringIntoView();
					e.Handled = true;
				}
			}
		}

		public ItemsControl Control
		{
			get { return (ItemsControl)GetValue( ControlProperty ); }
			set { SetValue( ControlProperty, value ); }
		}

		public object Item
		{
			get { return GetValue( ItemProperty ); }
			set { SetValue( ItemProperty, value ); }
		}

		public static readonly DependencyProperty ControlProperty =
			DependencyProperty.Register( "Control", typeof( ItemsControl ), typeof( ScrollIntoView ), new PropertyMetadata( null ) );

		public static readonly DependencyProperty ItemProperty =
			DependencyProperty.Register( "Item", typeof( object ), typeof( ScrollIntoView ), new PropertyMetadata( null ) );
	}
}