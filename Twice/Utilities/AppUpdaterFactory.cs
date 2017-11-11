using System.Threading.Tasks;
using Squirrel;

namespace Twice.Utilities
{
	internal class AppUpdaterFactory : IAppUpdaterFactory
	{
		public async Task<IAppUpdater> Construct( bool includePreReleases )
		{
			var mgr = await UpdateManager.GitHubUpdateManager( "https://github.com/TheSylence/Twice", null, null, null, includePreReleases );
			return new AppUpdater( mgr );
		}
	}
}