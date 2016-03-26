using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twice.ViewModels
{
	class CloseEventArgs : EventArgs
	{
		private CloseEventArgs( bool result )
		{
			Result = result;
		}

		public static readonly CloseEventArgs Ok = new CloseEventArgs( true );
		public static readonly CloseEventArgs Cancel = new CloseEventArgs( false );

		public readonly bool Result;
	}

	interface IViewController
	{
		event EventHandler<CloseEventArgs> CloseRequested;
	}
}
