using GalaSoft.MvvmLight.Messaging;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Twice.Messages;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
	internal class WaitMessageHandler
	{
		public WaitMessageHandler( IMessenger messenger )
		{
			messenger.Register<WaitMessage>( this, OnWaitMessage );
		}

		private void OnWaitMessage( WaitMessage msg )
		{
			if( msg.Start )
			{
				MainWindow = Application.Current.MainWindow;

				OldCursor = MainWindow.Cursor;
				MainWindow.Cursor = Cursors.Wait;
			}
			else
			{
				MainWindow.Cursor = OldCursor;
			}
		}

		private Window MainWindow;
		private Cursor OldCursor;
	}
}