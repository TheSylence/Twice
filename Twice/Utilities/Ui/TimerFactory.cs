using System;
using System.Windows.Threading;

namespace Twice.Utilities.Ui
{
	class TimerFactory : ITimerFactory
	{
		public ITimer Create( int timeout )
		{
			var toWrap = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds( timeout )
			};

			return new Timer( toWrap );
		}
	}
}