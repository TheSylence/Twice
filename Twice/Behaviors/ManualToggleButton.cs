using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class ManualToggleButton : BehaviorBase<Button>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.Click += AssociatedObject_Click;
		}

		private void AssociatedObject_Click( object sender, RoutedEventArgs e )
		{
			IsChecked = !IsChecked;
		}

		public static readonly DependencyProperty IsCheckedProperty =
			DependencyProperty.Register( "IsChecked", typeof(bool), typeof(ManualToggleButton), new PropertyMetadata( false ) );

		public bool IsChecked
		{
			get { return (bool)GetValue( IsCheckedProperty ); }
			set { SetValue( IsCheckedProperty, value ); }
		}
	}
}