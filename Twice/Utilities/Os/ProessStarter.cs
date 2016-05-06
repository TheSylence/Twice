using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Twice.Utilities.Os
{
	[ExcludeFromCodeCoverage]
	internal class ProcessStarter : IProcessStarter
	{
		public void Start( string proc )
		{
			Process.Start( proc );
		}
	}
}