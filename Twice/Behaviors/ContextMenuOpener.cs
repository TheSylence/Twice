using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class ContextMenuOpener : Behavior<Button>
	{
		protected override void OnAttached()
		{
			AssociatedObject.Click += AssociatedObject_Click;
		}

		private void AssociatedObject_Click( object sender, RoutedEventArgs e )
		{
			if( Element == null )
			{
				return;
			}

			Element.ContextMenu.IsOpen = true;
		}

		public FrameworkElement Element
		{
			get { return (FrameworkElement)GetValue( ElementProperty ); }
			set { SetValue( ElementProperty, value ); }
		}

		public static readonly DependencyProperty ElementProperty =
			DependencyProperty.Register( "Element", typeof( FrameworkElement ), typeof( ContextMenuOpener ), new PropertyMetadata( null ) );
	}
}