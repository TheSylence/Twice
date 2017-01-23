using Anotar.NLog;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Models.Media;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class Kernel : StandardKernel
	{
		public Kernel()
			: base( InjectionModules.ToArray() )
		{
			MigrateAppData();

			MediaExtractorRepository.Default.AddExtractor( new InstragramExtractor() );
			MediaExtractorRepository.Default.AddExtractor( new YoutubeExtractor() );
			//MediaExtractorRepository.Default.AddExtractor( new TwitterVideoExtractor() );

			MediaExtractorRepository.Default.AddExtractor( new GenericMediaExtractor() );
		}

		private static void MigrateAppData()
		{
			try
			{
#if !DEBUG
				var localAppDataFolder = Constants.IO.AppDataFolder;
				var roamingAppDataFolder = Constants.IO.RoamingAppDataFolder;

				if( System.IO.Directory.Exists( localAppDataFolder ) )
				{
					foreach( var file in System.IO.Directory.GetFiles( localAppDataFolder ) )
					{
						var targetFile = System.IO.Path.GetFileName( file );
						if( targetFile == null )
						{
							continue;
						}

						System.IO.File.Move( file, System.IO.Path.Combine( roamingAppDataFolder, targetFile ) );
					}

					System.IO.Directory.Delete( localAppDataFolder );
				}
#endif
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to migrate appdata", ex );
			}
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