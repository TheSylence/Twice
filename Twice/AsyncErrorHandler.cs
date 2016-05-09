using System;
using System.Diagnostics.CodeAnalysis;
using Anotar.NLog;
// ReSharper disable UnusedMember.Global

namespace Twice
{
	[ExcludeFromCodeCoverage]
	public static class AsyncErrorHandler
	{
		public static void HandleException( Exception exception )
		{
			LogTo.FatalException( "Exception in async code", exception );
			LogTo.Fatal( exception.StackTrace );
		}
	}
}