using NuGet;
using Squirrel;

namespace Twice.Utilities
{
	class AppRelease
	{
		public AppRelease( ReleaseEntry entry )
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