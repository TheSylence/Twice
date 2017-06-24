using System.Diagnostics.CodeAnalysis;
using Ninject.Modules;
using Twice.Views.Services;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class ServiceInjectionModule : NinjectModule

	{
		/// <summary>
		///     Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IViewServiceRepository>().To<ViewServiceRepository>().InSingletonScope();
		}
	}
}