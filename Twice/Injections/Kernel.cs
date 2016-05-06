using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ninject;
using Ninject.Modules;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class Kernel : StandardKernel
	{
		public Kernel()
			: base( InjectionModules.ToArray() )
		{
		}

		private static IEnumerable<INinjectModule> InjectionModules
		{
			get
			{
				yield return new ModelInjectionModule();
				yield return new ViewModelInjectionModule();
				yield return new ServiceInjectionModule();
				yield return new UtilitiyInjectionModule();
			}
		}
	}
}