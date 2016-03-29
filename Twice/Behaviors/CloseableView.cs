using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Interactivity;
using Twice.ViewModels;

namespace Twice.Behaviors
{
	[ExcludeFromCodeCoverage]
	internal class CloseableView : Behavior<Window>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			var controller = AssociatedObject.DataContext as IViewController;
			if( controller != null )
			{
				controller.CloseRequested += Controller_CloseRequested;
			}
		}

		private void Controller_CloseRequested( object sender, CloseEventArgs e )
		{
			try
			{
				AssociatedObject.DialogResult = e.Result;
			}
			catch( InvalidOperationException )
			{ }

			AssociatedObject.Close();
		}
	}
}