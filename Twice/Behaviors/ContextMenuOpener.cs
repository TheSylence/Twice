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
			if( Element?.ContextMenu != null )
			{
				Element.ContextMenu.IsOpen = true;
			}
		}

		public static readonly DependencyProperty ElementProperty =
			DependencyProperty.Register( "Element", typeof(FrameworkElement), typeof(ContextMenuOpener),
				new PropertyMetadata( null ) );

		public FrameworkElement Element
		{
			get { return (FrameworkElement)GetValue( ElementProperty ); }
			set { SetValue( ElementProperty, value ); }
		}
	}
}