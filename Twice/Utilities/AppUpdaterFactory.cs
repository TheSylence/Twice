using Squirrel;

namespace Twice.Utilities
{
	internal class AppUpdaterFactory : IAppUpdaterFactory
	{
		public IAppUpdater Construct( string url )
		{
			var mgr = new UpdateManager( url );
			return new AppUpdater( mgr );
		}
	}
}