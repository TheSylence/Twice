using Anotar.NLog;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Twice
{
	[ExcludeFromCodeCoverage]
	public static class AsyncErrorHandler
	{
		public static void HandleException( Exception exception )
		{
			LogTo.FatalException( "Exception in async code", exception );
		}
	}
}