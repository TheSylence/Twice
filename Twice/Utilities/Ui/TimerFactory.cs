using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
	internal class TimerFactory : ITimerFactory
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