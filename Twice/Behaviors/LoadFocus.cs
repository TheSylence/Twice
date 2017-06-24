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

		private void AssociatedObject_Loaded( object sender, RoutedEventArgs e )
		{
			FocusElement?.Focus();
		}

		private static void OnFocusElementChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var load = d as LoadFocus;
			load?.OnFocusElementChanged( e.NewValue as IInputElement );
		}

		private void OnFocusElementChanged( IInputElement inputElement )
		{
			inputElement?.Focus();
		}

		public static readonly DependencyProperty FocusElementProperty =
			DependencyProperty.Register( "FocusElement", typeof( IInputElement ), typeof( LoadFocus ), new PropertyMetadata( null, OnFocusElementChanged ) );

		public IInputElement FocusElement
		{
			get => (IInputElement)GetValue( FocusElementProperty );
			set => SetValue( FocusElementProperty, value );
		}
	}
}