using Ninject;
using System;
using System.Diagnostics.CodeAnalysis;
using Twice.Injections;

namespace Twice.Tests
{
	public interface ITypeResolver
	{
		object Resolve( Type type );
	}

	[ExcludeFromCodeCoverage]
	internal class NinjectTypeResolver : ITypeResolver
	{
		public NinjectTypeResolver()
		{
			Kernel = new Kernel();
		}

		public object Resolve( Type type )
		{
			return Kernel.Get( type );
		}

		private readonly IKernel Kernel;
	}
}