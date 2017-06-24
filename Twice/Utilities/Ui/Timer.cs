using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
	internal class Timer : ITimer
	{
		public Timer( DispatcherTimer timer )
		{
			Wrapped = timer;
		}

		public bool IsEnabled => Wrapped.IsEnabled;

		public void Start()
		{
			Wrapped.Start();
		}

		public void Stop()
		{
			Wrapped.Stop();
		}

		public event EventHandler Tick
		{
			add => Wrapped.Tick += value;
			remove => Wrapped.Tick -= value;
		}

		private readonly DispatcherTimer Wrapped;
	}
}