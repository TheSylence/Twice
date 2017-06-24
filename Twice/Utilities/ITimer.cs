using System;

namespace Twice.Utilities
{
	internal interface ITimer
	{
		void Start();

		void Stop();

		bool IsEnabled { get; }
		event EventHandler Tick;
	}
}