using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading.Tasks;

namespace Twice.Utilities
{
	internal class DispatcherHelperWrapper : IDispatcher
	{
		public void CheckBeginInvokeOnUI( Action action )
		{
			DispatcherHelper.CheckBeginInvokeOnUI( action );
		}

		public async Task RunAsync( Action action )
		{
			await DispatcherHelper.UIDispatcher.InvokeAsync( action );
		}
	}
}