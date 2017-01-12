using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Twice.Behaviors
{
	/// <summary>
	///  Executes a command when the Return or Enter-Key was pressed in a TextBox 
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal class CommandOnReturn : BehaviorBase<TextBox>
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

		/// <summary>
		///  The command to execute 
		/// </summary>
		public ICommand Command
		{
			get { return (ICommand)GetValue( CommandProperty ); }
			set { SetValue( CommandProperty, value ); }
		}

		/// <summary>
		///  Parameter that will be passed to the command 
		/// </summary>
		public object CommandParameter
		{
			get { return GetValue( CommandParameterProperty ); }
			set { SetValue( CommandParameterProperty, value ); }
		}

		public static readonly DependencyProperty CommandParameterProperty =
							DependencyProperty.Register( "CommandParameter", typeof( object ), typeof( CommandOnReturn ),
				new PropertyMetadata( null ) );

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register( "Command", typeof( ICommand ), typeof( CommandOnReturn ), new PropertyMetadata( null ) );
	}
}