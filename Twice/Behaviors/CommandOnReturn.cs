using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class CommandOnReturn : Behavior<TextBox>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
		}

		private void AssociatedObject_PreviewKeyDown( object sender, KeyEventArgs e )
		{
			if( e.Key != Key.Return && e.Key != Key.Enter )
			{
				return;
			}

			if( Command?.CanExecute( CommandParameter ) == true )
			{
				Command.Execute( CommandParameter );
				e.Handled = true;
			}
		}

		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.Register( "CommandParameter", typeof(object), typeof(CommandOnReturn),
				new PropertyMetadata( null ) );

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register( "Command", typeof(ICommand), typeof(CommandOnReturn), new PropertyMetadata( null ) );

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