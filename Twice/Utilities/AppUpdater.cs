using System.Threading.Tasks;
using Squirrel;

namespace Twice.Utilities
{
	internal class AppUpdater : IAppUpdater
	{
		public AppUpdater( UpdateManager mgr )
		{
			Manager = mgr;
		}

		public void Dispose()
		{
			Manager.Dispose();
		}

		public async Task<AppRelease> UpdateApp()
		{
			var release = await Manager.UpdateApp();
			return new AppRelease( release );
		}

		private readonly UpdateManager Manager;
	}
}