using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twice.Tests
{
	[ExcludeFromCodeCoverage]
	internal static class ExceptionAssert
	{
		public static TException Catch<TException>( Action action ) where TException : Exception
		{
			try
			{
				action();
				return null;
			}
			catch( TException ex )
			{
				return ex;
			}
		}

		public static void Throws<TException>( Action action ) where TException : Exception
		{
			try
			{
				action();
				Assert.Fail( $"Expected exception of type {typeof( TException )} but none was thrown." );
			}
			catch( TException )
			{
			}
			catch( Exception ex )
			{
				Assert.Fail( $"Expected exception of type {typeof( TException )} but {ex.GetType()} was thrown." );
			}
		}
	}
}