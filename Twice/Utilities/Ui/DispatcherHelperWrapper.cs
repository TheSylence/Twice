using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Fody;
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

		[ConfigureAwait( false )]
		public async Task RunAsync( Action action )
		{
			await DispatcherHelper.UIDispatcher.InvokeAsync( action );
		}
	}
}