using System;
using System.Threading.Tasks;

namespace Twice.Utilities
{
	interface IAppUpdater : IDisposable
	{
		Task<AppRelease> UpdateApp();
	}
}