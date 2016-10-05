using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace Twice.Behaviors
{
	/// <summary>
	///     Executes a command when a FrameworkElement has been clicked.
	/// </summary>
	/// <remarks>Essentially this is a nicer way than using EventToCommand for the click command</remarks>
	[ExcludeFromCodeCoverage]
	internal class ClickHandler : BehaviorBase<FrameworkElement>
	{
		protected override void OnAttached()
		{
			AssociatedObject.MouseDown += AssociatedObject_MouseDown;
		}

		private void AssociatedObject_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if( e.LeftButton != MouseButtonState.Pressed )
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
			DependencyProperty.Register( "CommandParameter", typeof(object), typeof(ClickHandler), new PropertyMetadata( null ) );

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register( "Command", typeof(ICommand), typeof(ClickHandler), new PropertyMetadata( null ) );

		/// <summary>
		///     The command to execute
		/// </summary>
		public ICommand Command
		{
			get { return (ICommand)GetValue( CommandProperty ); }
			set { SetValue( CommandProperty, value ); }
		}

		/// <summary>
		///     Parameter that will be passed to the command
		/// </summary>
		public object CommandParameter
		{
			get { return GetValue( CommandParameterProperty ); }
			set { SetValue( CommandParameterProperty, value ); }
		}
	}
}