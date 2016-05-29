using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class ClickHandler : Behavior<FrameworkElement>
	{
		protected override void OnAttached()
		{
			AssociatedObject.MouseDown += AssociatedObject_MouseDown;
		}

		private void AssociatedObject_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if( e.LeftButton == MouseButtonState.Pressed )
			{
				if( Command?.CanExecute( CommandParameter ) == true )
				{
					Command.Execute( CommandParameter );
					e.Handled = true;
				}
			}
		}

		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.Register( "CommandParameter", typeof(object), typeof(ClickHandler), new PropertyMetadata( null ) );

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register( "Command", typeof(ICommand), typeof(ClickHandler), new PropertyMetadata( null ) );

		public ICommand Command
		{
			get { return (ICommand)GetValue( CommandProperty ); }
			set { SetValue( CommandProperty, value ); }
		}

		public object CommandParameter
		{
			get { return (object)GetValue( CommandParameterProperty ); }
			set { SetValue( CommandParameterProperty, value ); }
		}
	}
}