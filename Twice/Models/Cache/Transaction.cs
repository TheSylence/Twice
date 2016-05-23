using System;
using System.Data.SQLite;

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

		private readonly SQLiteConnection Connection;
		private bool Done;
	}
}