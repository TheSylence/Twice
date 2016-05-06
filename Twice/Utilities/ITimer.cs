using System;

namespace Twice.Utilities
{
	interface ITimer
	{
		event EventHandler Tick;
		void Start();
		void Stop();
		bool IsEnabled { get; }
	}
}