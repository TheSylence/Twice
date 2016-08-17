using Ninject.Modules;
using System.Diagnostics.CodeAnalysis;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Scheduling;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Streaming;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class ModelInjectionModule : NinjectModule
	{
		/// <summary>
		///     Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<ICache>().ToProvider<CacheProvider>().InSingletonScope();

			Bind<ITwitterContextList>().ToProvider<TwitterContextListProvider>().InSingletonScope();
			Bind<IConfig>().ToProvider<ConfigurationProvider>().InSingletonScope();
			Bind<IStatusMuter>().To<StatusMuter>();
			Bind<IStreamingRepository>().To<StreamingRepository>().InSingletonScope();
			Bind<IColumnDefinitionList>().ToProvider<ColumnDefinitionListProvider>().InSingletonScope();
			Bind<ITwitterConfiguration>().To<TwitterConfiguration>().InSingletonScope();
			Bind<IScheduler>().ToProvider<SchedulerProvider>().InSingletonScope();
		}
	}
}