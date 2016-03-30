using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Twice
{
	[ExcludeFromCodeCoverage]
	internal static class Obscurity
	{
		internal static string Deobscure( string input )
		{
			var dec = new string( input.ToCharArray().Reverse().ToArray() );
			var bytes = Convert.FromBase64String( dec );
			return Encoding.ASCII.GetString( bytes );
		}
	}
}