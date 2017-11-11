using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Twice.Behaviors
{
	/// <summary>
	///     Allows pasting image data from clipboard to a textbox.
	/// </summary>
	internal class ImageDataHandler : BehaviorBase<TextBox>
	{
		protected override void OnAttached()
		{
			CommandManager.AddPreviewCanExecuteHandler( AssociatedObject, OnPreviewCanExecute );
			CommandManager.AddPreviewExecutedHandler( AssociatedObject, OnPreviewExecuted );
		}

		protected override void OnCleanup()
		{
			CommandManager.RemovePreviewCanExecuteHandler( AssociatedObject, OnPreviewCanExecute );
			CommandManager.RemovePreviewExecutedHandler( AssociatedObject, OnPreviewExecuted );
		}

		private void OnPreviewCanExecute( object sender, CanExecuteRoutedEventArgs e )
		{
			if( e.Command == ApplicationCommands.Paste )
			{
				e.CanExecute = true;
				e.Handled = true;
			}
		}

		private void OnPreviewExecuted( object sender, ExecutedRoutedEventArgs e )
		{
			if( e.Command != ApplicationCommands.Paste )
			{
				return;
			}

			if( AttachCommand == null )
			{
				return;
			}

			if( !Clipboard.ContainsImage() )
			{
				return;
			}

			var image = Clipboard.GetImage();
			if( image == null )
			{
				return;
			}

			var fileName = Path.GetTempFileName();

			using( var stream = File.OpenWrite( fileName ) )
			{
				var encoder = new PngBitmapEncoder();
				encoder.Frames.Add( BitmapFrame.Create( image ) );
				encoder.Save( stream );
			}

			if( AttachCommand.CanExecute( fileName ) )
			{
				AttachCommand.Execute( fileName );
			}
		}

		public static readonly DependencyProperty AttachCommandProperty =
			DependencyProperty.Register( "AttachCommand", typeof( ICommand ), typeof( ImageDataHandler ), new PropertyMetadata( null ) );

		public ICommand AttachCommand
		{
			get => (ICommand)GetValue( AttachCommandProperty );
			set => SetValue( AttachCommandProperty, value );
		}
	}
}