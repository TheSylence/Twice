using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

namespace Twice.Behaviors
{
	/// <summary>
	///  Selects the whole content of a TextBox when it was double clicked. 
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal class SelectOnDoubleClick : BehaviorBase<TextBox>
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