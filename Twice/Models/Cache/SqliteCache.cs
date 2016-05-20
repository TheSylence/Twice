using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using Fody;
using Newtonsoft.Json;

namespace Twice.Models.Cache
{
	[ConfigureAwait( false )]
	internal partial class SqliteCache : ICache
	{
		public SqliteCache( string connectionString )
		{
			Connection = new SQLiteConnection( connectionString );
			Connection.Open();

			Init();
		}

		public SqliteCache( SQLiteConnection connection )
		{
			Connection = connection;

			Init();
		}

		private async Task Cleanup()
		{
			string[] tables =
			{
				"Users", "TwitterConfig", "Hashtags", "Statuses"
			};

			ulong now = SqliteHelper.GetDateValue( DateTime.Now );

			foreach( var table in tables )
			{
				using( var cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = $"DELETE FROM {table} WHERE Expires < @now;";
					cmd.AddParameter( "now", now );
					await cmd.ExecuteNonQueryAsync();
				}
			}
		}

		private void Init()
		{
			foreach( var qry in GetDdlQueries() )
			{
				using( var cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = qry;
					cmd.ExecuteNonQuery();
				}
			}
		}

		public async Task AddHashtag( string hashTag )
		{
			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "INSERT INTO Hashtags (Tag, Expires) VALUES (@tag, @expires);";
				cmd.AddParameter( "tag", hashTag );
				cmd.AddParameter( "expires", SqliteHelper.GetDateValue( DateTime.Now.Add( HashtagExpiration ) ) );

				await cmd.ExecuteNonQueryAsync();
			}
		}

		public async Task AddUser( UserCacheEntry user )
		{
			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "INSERT INTO Users (Id, UserName, UserData, Expires) VALUES " +
								"(@userId, @userName, @json, @expires);";

				cmd.AddParameter( "userId", user.UserId );
				cmd.AddParameter( "userName", user.UserName );
				cmd.AddParameter( "json", user.Data );
				cmd.AddParameter( "expires", SqliteHelper.GetDateValue( DateTime.Now.Add( UserExpiration ) ) );

				await cmd.ExecuteNonQueryAsync();
			}
		}

		public void Dispose()
		{
			Connection.Dispose();
		}

		public async Task<IEnumerable<string>> GetKnownHashtags()
		{
			await Cleanup();

			List<string> tags = new List<string>();
			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT Tag FROM Hashtags;";
				using( var reader = await cmd.ExecuteReaderAsync() )
				{
					while( await reader.ReadAsync() )
					{
						tags.Add( await reader.GetFieldValueAsync<string>( 0 ) );
					}
				}
			}

			return tags;
		}

		public async Task<IEnumerable<UserCacheEntry>> GetKnownUsers()
		{
			await Cleanup();

			List<UserCacheEntry> result = new List<UserCacheEntry>();

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT Id, UserName, UserData FROM Users;";

				using( var reader = await cmd.ExecuteReaderAsync() )
				{
					while( await reader.ReadAsync() )
					{
						result.Add( await UserCacheEntry.Read( reader ) );
					}
				}
			}

			return result;
		}

		public async Task<LinqToTwitter.Configuration> ReadTwitterConfig()
		{
			await Cleanup();

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT Data FROM TwitterConfig;";
				var json = await cmd.ExecuteScalarAsync() as string;

				if( string.IsNullOrEmpty( json ) )
				{
					return null;
				}

				return JsonConvert.DeserializeObject<LinqToTwitter.Configuration>( json );
			}
		}

		public async Task SaveTwitterConfig( LinqToTwitter.Configuration cfg )
		{
			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "DELETE FROM TwitterConfig;";
				await cmd.ExecuteNonQueryAsync();
			}
			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "INSERT INTO TwitterConfig (Data, Expires) VALUES(@json, @expires);";
				cmd.AddParameter( "json", JsonConvert.SerializeObject( cfg ) );
				cmd.AddParameter( "expires", SqliteHelper.GetDateValue( DateTime.Now.Add( TwitterConfigExpiration ) ) );

				await cmd.ExecuteNonQueryAsync();
			}
		}

		private readonly SQLiteConnection Connection;
		private readonly TimeSpan HashtagExpiration = TimeSpan.FromDays( 30 );
		private readonly TimeSpan TwitterConfigExpiration = TimeSpan.FromDays( 1 );
		private readonly TimeSpan UserExpiration = TimeSpan.FromDays( 14 );
	}
}