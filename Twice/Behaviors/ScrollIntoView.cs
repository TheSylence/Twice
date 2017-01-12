using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Twice.Behaviors
{
	/// <summary>
	///  Scrolls an item into view, when a button is button is pressed. 
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal class ScrollIntoView : BehaviorBase<Button>
	{
		protected override void OnAttached()
		{
			AssociatedObject.Click += AssociatedObject_Click;
		}

		private void AssociatedObject_Click( object sender, RoutedEventArgs e )
		{
			if( Item == null || Control == null )
			{
				return;
			}

			var element = Control.ItemContainerGenerator.ContainerFromItem( Item ) as FrameworkElement;
			if( element == null )
			{
				return;
			}

			element.BringIntoView();
			e.Handled = true;
		}

		/// <summary>
		///  ItemControl that contains the item. 
		/// </summary>
		public ItemsControl Control
		{
			get { return (ItemsControl)GetValue( ControlProperty ); }
			set { SetValue( ControlProperty, value ); }
		}

		/// <summary>
		///  The item that should be visible after the button press 
		/// </summary>
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