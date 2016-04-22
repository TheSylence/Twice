using System;
using System.Threading.Tasks;
using Twice.Utilities;
using Twice.Utilities.Ui;

namespace Twice.Tests
{
	internal class SyncDispatcher : IDispatcher
	{
		public void CheckBeginInvokeOnUI( Action action )
		{
			action();
		}

		public Task RunAsync( Action action )
		{
			action();
			return Task.CompletedTask;
		}
	}
}