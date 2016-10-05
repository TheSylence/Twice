using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Twice.Controls;
using Twice.ViewModels.Columns;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class ColumnHeaderHandler : BehaviorBase<ColumnHeader>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
		}

		private void AssociatedObject_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
		{
			ActionDispatcher?.OnHeaderClicked();
		}

		public static readonly DependencyProperty ActionDispatcherProperty =
			DependencyProperty.Register( "ActionDispatcher", typeof(IColumnActionDispatcher), typeof(ColumnHeaderHandler),
				new PropertyMetadata( null ) );

		public IColumnActionDispatcher ActionDispatcher
		{
			get { return (IColumnActionDispatcher)GetValue( ActionDispatcherProperty ); }
			set { SetValue( ActionDispatcherProperty, value ); }
		}
	}
}