using System.Threading.Tasks;

namespace Twice.Utilities
{
	internal interface IAppUpdaterFactory
	{
		Task<IAppUpdater> Construct( bool includePreReleases );
	}
}