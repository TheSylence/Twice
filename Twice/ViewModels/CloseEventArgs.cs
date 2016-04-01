using System;

namespace Twice.ViewModels
{
	internal class CloseEventArgs : EventArgs
	{
		private CloseEventArgs( bool result )
		{
			Result = result;
		}

		public static readonly CloseEventArgs Cancel = new CloseEventArgs( false );
		public static readonly CloseEventArgs Ok = new CloseEventArgs( true );
		public readonly bool Result;
	}
}