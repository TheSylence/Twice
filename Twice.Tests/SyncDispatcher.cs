using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Twice.Utilities.Ui;

namespace Twice.Tests
{
	[ExcludeFromCodeCoverage]
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