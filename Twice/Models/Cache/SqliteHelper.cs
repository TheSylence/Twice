using System;
using System.Data.SQLite;

namespace Twice.Models.Cache
{
	internal static class SqliteHelper
	{
		public static void AddParameter( this SQLiteCommand cmd, string name, object value )
		{
			var p = cmd.CreateParameter();
			p.ParameterName = name;
			p.Value = value;
			cmd.Parameters.Add( p );
		}

		public static ulong GetDateValue( DateTime dt )
		{
			var span = dt - EpochStart;
			return (ulong)span.TotalSeconds;
		}

		private static readonly DateTime EpochStart = new DateTime( 1970, 1, 1, 0, 0, 0 );
	}
}