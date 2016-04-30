using System;

namespace Twice.Utilities
{
	interface ITimer
	{
		void Start();
		void Stop();
		event EventHandler Tick;
		bool IsEnabled { get; }
	}
}