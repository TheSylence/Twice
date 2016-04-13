using Ninject.Modules;
using System.Diagnostics.CodeAnalysis;
using Twice.Utilities;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class UtilitiyInjectionModule : NinjectModule
	{
		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IColorProvider>().To<ColorProvider>();
			Bind<ILanguageProvider>().To<LanguageProvider>();
			Bind<IDispatcher>().To<DispatcherHelperWrapper>().InSingletonScope();
			Bind<IFileSystem>().To<FileSystem>();
		}
	}
}