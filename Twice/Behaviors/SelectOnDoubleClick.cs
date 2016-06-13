using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Twice.Behaviors
{
	internal class SelectOnDoubleClick : Behavior<TextBox>
	{
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
		}

		private void AssociatedObject_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
		{
			AssociatedObject.SelectAll();
		}
	}
}