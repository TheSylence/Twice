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
			SqliteHelper.ExecuteNonQuerySafe( "BEGIN EXCLUSIVE TRANSACTION;", Connection );

			// Capture current stack trace in debug so we know where a transaction was started but
			// not stopped
			CaptureStackTrace();
		}

		public void Commit()
		{
			Done = true;
			SqliteHelper.ExecuteNonQuerySafe( "COMMIT TRANSACTION;", Connection );
		}

		public void Dispose()
		{
			if( !Done )
			{
				SqliteHelper.ExecuteNonQuerySafe( "ROLLBACK TRANSACTION;", Connection );
			}
		}

		[Conditional( "DEBUG" )]
		private static void CaptureStackTrace()
		{
			LastCreationTrace = new StackTrace();
		}

		// ReSharper disable once NotAccessedField.Local => Used for debugging
		private static StackTrace LastCreationTrace;

		private readonly SQLiteConnection Connection;
		private bool Done;
	}
}