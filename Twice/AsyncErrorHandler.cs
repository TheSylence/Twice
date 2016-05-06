using System;
using System.Diagnostics.CodeAnalysis;
using Anotar.NLog;

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