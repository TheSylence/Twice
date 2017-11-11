using System;
using System.Data.SQLite;
using Anotar.NLog;

namespace Twice.Models.Cache
{
	/// <summary>
	///     Helper methods for SQLite
	/// </summary>
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

		/// <summary>
		///     Tries to execute a query against a database, catches exceptions and tries to execute
		///     query multiple times.
		/// </summary>
		/// <param name="query"> Query to execute </param>
		/// <param name="connection"> Connection to execute query on </param>
		/// <param name="maxTries"> Max. attemps before failing </param>
		internal static void ExecuteNonQuerySafe( string query, SQLiteConnection connection, int maxTries = 3 )
		{
			SQLiteException lastException = null;

			for( int i = 0; i < maxTries; ++i )
			{
				try
				{
					using( var cmd = connection.CreateCommand() )
					{
						cmd.CommandText = query;
						cmd.ExecuteNonQuery();
					}

					return;
				}
				catch( SQLiteException ex )
				{
					lastException = ex;
				}
			}

			LogTo.WarnException( $"Failed to excute query after {maxTries} attemps: {query}", lastException );
		}

		private static readonly DateTime EpochStart = new DateTime( 1970, 1, 1, 0, 0, 0 );
	}
}