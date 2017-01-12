using NuGet;
using Squirrel;

namespace Twice.Utilities
{
	/// <summary>
	///  Wrapper for ReleaseEntry 
	/// </summary>
	internal class AppRelease
	{
		public AppRelease( IReleaseEntry entry )
		{
			Version = entry.Version;
		}

		internal AppRelease( SemanticVersion version )
		{
			Version = version;
		}

		public SemanticVersion Version { get; }
	}
}