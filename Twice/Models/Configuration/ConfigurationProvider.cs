using Ninject.Activation;
using System.Diagnostics.CodeAnalysis;

namespace Twice.Models.Configuration
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
			return Config ?? ( Config = new Config( Constants.IO.ConfigFileName ) );
		}

		private static IConfig Config;
	}
}