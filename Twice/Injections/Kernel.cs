using System.Diagnostics.CodeAnalysis;
using Ninject;
using Ninject.Infrastructure;
using Ninject.Modules;
using Ninject.Planning.Bindings;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class Kernel : StandardKernel
	{
		public Kernel( params INinjectModule[] modules )
			: base( modules )
		{
		}

		public override void AddBinding( IBinding binding )
		{
			binding.ScopeCallback = StandardScopeCallbacks.Singleton;
			base.AddBinding( binding );
		}
	}
}