using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Twice.Utilities;

namespace Twice
{
	internal static class Extensions
	{
		public static void AddRange<T>( this ICollection<T> collection, IEnumerable<T> range )
		{
			var scol = collection as SmartCollection<T>;
			var list = collection as List<T>;

			if( scol != null )
			{
				scol.AddRange( range );
			}
			else if( list != null )
			{
				list.AddRange( range );
			}
			else
			{
				foreach( var item in range )
				{
					collection.Add( item );
				}
			}
		}

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

		public static string GetReason( this Exception ex )
		{
			while( ex is AggregateException )
			{
				ex = ex.InnerException;
			}

			var tex = ex as TargetInvocationException;
			if( tex != null )
			{
				return tex.InnerException.GetReason();
			}

			return ex?.Message;
		}
	}
}