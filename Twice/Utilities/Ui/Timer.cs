using System;
using System.Windows.Threading;

namespace Twice.Utilities.Ui
{
	class Timer : ITimer
	{
		public Timer( DispatcherTimer timer )
		{
			Wrapped = timer;
		}

		public event EventHandler Tick
		{
			add { Wrapped.Tick += value; }
			remove { Wrapped.Tick -= value; }
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

		private readonly DispatcherTimer Wrapped;
	}
}