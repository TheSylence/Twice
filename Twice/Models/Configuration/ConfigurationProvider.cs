using Ninject.Activation;

namespace Twice.Models.Configuration
{
	internal class ConfigurationProvider : Provider<IConfig>
	{
		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// The created instance.
		/// </returns>
		protected override IConfig CreateInstance( IContext context )
		{
			if( Config == null )
			{
				Config = new Config( Constants.IO.ConfigFileName );
			}

			return Config;
		}

		private static IConfig Config;
	}
}