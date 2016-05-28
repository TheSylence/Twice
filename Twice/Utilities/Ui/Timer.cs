using System;
using System.Windows.Threading;

namespace Twice.Utilities.Ui
{
	internal class Timer : ITimer
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

		public void Start()
		{
			Wrapped.Start();
		}

		public void Stop()
		{
			Wrapped.Stop();
		}

		public bool IsEnabled => Wrapped.IsEnabled;
		private readonly DispatcherTimer Wrapped;
	}
}