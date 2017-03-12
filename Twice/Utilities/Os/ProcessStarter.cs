using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;

namespace Twice.Utilities.Os
{
	[ExcludeFromCodeCoverage]
	internal class ProcessStarter : IProcessStarter
	{
		public void Start( string proc )
		{
			Process.Start( proc );
		}

		public void Restart()
		{
			var procName = Assembly.GetExecutingAssembly().CodeBase;
			var startInfo = new ProcessStartInfo(procName, Constants.IgnoreMutexFlag );

			Process.Start(startInfo);
			Application.Current.Shutdown(0);
		}
	}
}