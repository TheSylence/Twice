using System;

namespace Twice.Utilities
{
	internal interface ITimer
	{
		event EventHandler Tick;

		void Start();

		void Stop();

		bool IsEnabled { get; }
	}
}