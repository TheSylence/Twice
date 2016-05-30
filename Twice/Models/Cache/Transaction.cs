using System;
using System.Data.SQLite;
using System.Diagnostics;

namespace Twice.Models.Cache
{
	internal class Transaction : IDisposable
	{
		public Transaction( SQLiteConnection connection )
		{
			Connection = connection;

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "BEGIN EXCLUSIVE TRANSACTION;";
				cmd.ExecuteNonQuery();
			}

			// Capture current stack trace in debug so we know
			// where a transaction was started but not stopped
			CaptureStackTrace();
		}

		public void Commit()
		{
			Done = true;
			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "COMMIT TRANSACTION;";
				cmd.ExecuteNonQuery();
			}
		}

		[Conditional( "DEBUG" )]
		private static void CaptureStackTrace()
		{
			LastCreationTrace = new StackTrace();
		}

		public void Dispose()
		{
			if( !Done )
			{
				using( var cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "ROLLBACK TRANSACTION;";
					cmd.ExecuteNonQuery();
				}
			}
		}

		// ReSharper disable once NotAccessedField.Local => Used for debugging
		private static StackTrace LastCreationTrace;
		private readonly SQLiteConnection Connection;
		private bool Done;
	}
}