using System;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Twice.Models.Cache
{
	internal class Database : IDatabase, IDisposable
	{
		public Database( string fileName )
		{
			var sb = new SQLiteConnectionStringBuilder
			{
				DataSource = fileName,
				FailIfMissing = false,
				ForeignKeys = false
			};

			Connection = new SQLiteConnection( sb.ToString() );
			Connection.Open();
		}

		private static void AddParameter( SQLiteCommand cmd, string name, object value )
		{
			var p = cmd.CreateParameter();
			p.Value = value;
			p.ParameterName = name;
			cmd.Parameters.Add( p );
		}

		public Task<bool> Exists( string key )
		{
			throw new NotImplementedException();
		}

		public async Task Insert( string key, string data, DateTime? expiryDate = null )
		{
			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "INSERT INTO DataCache (Key, Data, Expires) VALUES (?,?,?);";
				AddParameter( cmd, "Key", key );
				AddParameter( cmd, "Data", data );
				AddParameter( cmd, "Expires", expiryDate );

				await cmd.ExecuteNonQueryAsync();
			}
		}

		public Task<string> Read( string key )
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			Connection?.Dispose();
		}

		private readonly SQLiteConnection Connection;
	}
}