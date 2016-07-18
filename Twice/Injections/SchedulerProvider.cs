using Ninject.Activation;
using System.Diagnostics.CodeAnalysis;
using Ninject;
using Twice.Models.Scheduling;
using Twice.Models.Twitter;

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
			var contextList = context.Kernel.Get<ITwitterContextList>();
			var config = context.Kernel.Get<ITwitterConfiguration>();

			return new Scheduler( Constants.IO.SchedulerFileName, contextList, config );
		}
	}
}