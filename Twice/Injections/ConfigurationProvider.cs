using System.Diagnostics.CodeAnalysis;
using Ninject.Activation;
using Twice.Models.Configuration;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class ConfigurationProvider : Provider<IConfig>
	{
		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		protected override IConfig CreateInstance( IContext context )
		{
			return new Config( Constants.IO.ConfigFileName );
		}
	}
}