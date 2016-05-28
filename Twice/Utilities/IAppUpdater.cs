using System;
using System.Threading.Tasks;

namespace Twice.Utilities
{
	internal interface IAppUpdater : IDisposable
	{
		Task<AppRelease> UpdateApp();
	}
}