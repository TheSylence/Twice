using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
	internal class WaitOperation : IDisposable
	{
		public WaitOperation()
		{
			MainWindow = Application.Current.MainWindow;

			OldCursor = MainWindow.Cursor;
			MainWindow.Cursor = Cursors.Wait;
		}

		public void Dispose()
		{
			MainWindow.Cursor = OldCursor;

			GC.SuppressFinalize( this );
		}

		private readonly Window MainWindow;
		private readonly Cursor OldCursor;
	}
}