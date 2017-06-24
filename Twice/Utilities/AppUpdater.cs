using System.Threading.Tasks;
using Fody;
using Squirrel;

namespace Twice.Utilities
{
	/// <summary>
	///     Wrapper for an UpdateManager
	/// </summary>
	[ConfigureAwait( false )]
	internal class AppUpdater : IAppUpdater
	{
		public AppUpdater( UpdateManager mgr )
		{
			Manager = mgr;
		}

		public async Task<AppRelease> UpdateApp()
		{
			var release = await Manager.UpdateApp();
			return new AppRelease( release );
		}

		public void Dispose()
		{
			Manager.Dispose();
		}

		private readonly UpdateManager Manager;
	}
}