using System.Diagnostics.CodeAnalysis;
using Ninject.Modules;
using Twice.Utilities;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	class UtilitiyInjectionModule : NinjectModule
	{
		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IColorProvider>().To<ColorProvider>();
			Bind<ILanguageProvider>().To<LanguageProvider>();
		}
	}
}