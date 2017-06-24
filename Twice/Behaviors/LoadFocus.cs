using System.Windows;
using System.Windows.Controls;

namespace Twice.Behaviors
{
	internal class LoadFocus : BehaviorBase<UserControl>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.Loaded += AssociatedObject_Loaded;
		}

		private static void OnFocusElementChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var load = d as LoadFocus;
			load?.OnFocusElementChanged( e.NewValue as IInputElement );
		}

		private void AssociatedObject_Loaded( object sender, RoutedEventArgs e )
		{
			FocusElement?.Focus();
		}

		private void OnFocusElementChanged( IInputElement inputElement )
		{
			inputElement?.Focus();
		}

		public IInputElement FocusElement
		{
			get { return (IInputElement)GetValue( FocusElementProperty ); }
			set { SetValue( FocusElementProperty, value ); }
		}

		public static readonly DependencyProperty FocusElementProperty =
			DependencyProperty.Register( "FocusElement", typeof( IInputElement ), typeof( LoadFocus ), new PropertyMetadata( null, OnFocusElementChanged ) );
	}
}