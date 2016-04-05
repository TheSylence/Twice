using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ninject;
using Ninject.Infrastructure;
using Ninject.Modules;
using Ninject.Planning.Bindings;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class Kernel : StandardKernel
	{
		public Kernel()
			: base( InjectionModules.ToArray() )
		{
		}

		public override void AddBinding( IBinding binding )
		{
			binding.ScopeCallback = StandardScopeCallbacks.Singleton;
			base.AddBinding( binding );
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