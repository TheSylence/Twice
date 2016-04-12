using Ninject;
using System;
using Twice.Injections;

namespace Twice.Tests
{
	internal interface ITypeResolver
	{
		object Resolve( Type type );
	}

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