using System;
using System.Collections.Generic;
using System.Linq;

namespace Twice
{
	internal static class Extensions
	{
		public static DateTime AsUnixTimestamp( this ulong timetamp )
		{
			DateTime dtDateTime = new DateTime( 1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc );
			dtDateTime = dtDateTime.AddSeconds( timetamp ).ToLocalTime();
			return dtDateTime;
		}

		public static bool Compare<T>( this IEnumerable<T> value, IEnumerable<T> other )
		{
			var b = other as T[] ?? other.ToArray();
			var a = value as T[] ?? value.ToArray();

			return a.Length == b.Length
			       && !a.Except( b ).Any()
			       && !b.Except( a ).Any();
		}
	}
}