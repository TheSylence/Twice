using Ninject.Modules;
using System.Diagnostics.CodeAnalysis;
using Twice.Services.Views;

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