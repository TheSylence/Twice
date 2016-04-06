using System.Collections.Generic;
using System.Linq;

namespace Twice
{
	internal static class Extensions
	{
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