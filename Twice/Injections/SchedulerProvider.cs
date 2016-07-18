using Ninject.Activation;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Scheduling;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class SchedulerProvider : Provider<IScheduler>
	{
		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		protected override IScheduler CreateInstance( IContext context )
		{
			return new Scheduler( Constants.IO.SchedulerFileName );
		}
	}
}