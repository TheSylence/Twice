using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
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