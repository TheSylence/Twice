using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Newtonsoft.Json;
using Twice.Models.Twitter;

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
				"Users", "TwitterConfig", "Hashtags", "Statuses", "Messages"
			};

			ulong now = SqliteHelper.GetDateValue( DateTime.Now );

			await Semaphore.WaitAsync( SemaphoreWait );
			try
			{
				using( var tx = new Transaction( Connection ) )
				{
					foreach( var table in tables )
					{
						using( var cmd = Connection.CreateCommand() )
						{
							cmd.CommandText = $"DELETE FROM {table} WHERE Expires < @now;";
							cmd.AddParameter( "now", now );
							await cmd.ExecuteNonQueryAsync();
						}
					}

					tx.Commit();
				}
			}
			finally
			{
				Semaphore.Release();
			}
		}

		private void Init()
		{
			foreach( var qry in GetDdlQueries().Concat( GetInitQueries() ) )
			{
				using( var cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = qry;
					cmd.ExecuteNonQuery();
				}
			}
		}

		public async Task AddHashtags( IList<string> hashTags )
		{
			if( !hashTags.Any() )
			{
				return;
			}

			await Semaphore.WaitAsync( SemaphoreWait );
			try
			{
				using( var tx = new Transaction( Connection ) )
				{
					using( var cmd = Connection.CreateCommand() )
					{
						cmd.CommandText = "INSERT INTO Hashtags (Tag, Expires) VALUES ";
						cmd.CommandText += string.Join( ",", hashTags.Select( ( h, i ) =>
						{
							// ReSharper disable AccessToDisposedClosure
							cmd.AddParameter( $"tag{i}", h );

							// ReSharper restore AccessToDisposedClosure

							return $"( @tag{i}, @expires )";
						} ) );

						cmd.AddParameter( "expires", SqliteHelper.GetDateValue( DateTime.Now.Add( Constants.Cache.HashtagExpiration ) ) );

						await cmd.ExecuteNonQueryAsync();
					}

					tx.Commit();
				}
			}
			finally
			{
				Semaphore.Release();
			}
		}

		public async Task AddMessages( IList<MessageCacheEntry> messages )
		{
			await Semaphore.WaitAsync( SemaphoreWait );
			try
			{
				using( var tx = new Transaction( Connection ) )
				{
					int count = messages.Count;
					const int batchSize = 100;
					int runsNeeded = (int)Math.Ceiling( count / (float)batchSize );

					for( int batchIdx = 0; batchIdx < runsNeeded; ++batchIdx )
					{
						var items = messages.Skip( batchIdx * batchSize ).Take( batchSize );

						using( var cmd = Connection.CreateCommand() )
						{
							cmd.CommandText = "INSERT OR REPLACE INTO Messages (Id, Sender, Recipient, Data, Expires) VALUES ";

							cmd.CommandText += string.Join( ",", items.Select( ( s, i ) =>
							{
								// ReSharper disable AccessToDisposedClosure
								cmd.AddParameter( $"id{i}", s.Id );
								cmd.AddParameter( $"sender{i}", s.Sender );
								cmd.AddParameter( $"recipient{i}", s.Recipient );
								cmd.AddParameter( $"json{i}", s.Data );

								// ReSharper restore AccessToDisposedClosure

								return $"( @id{i}, @sender{i}, @recipient{i}, @json{i}, @expires )";
							} ) );

							cmd.AddParameter( "expires", SqliteHelper.GetDateValue( DateTime.Now.Add( Constants.Cache.MessageExpiration ) ) );
							await cmd.ExecuteNonQueryAsync();
						}
					}

					tx.Commit();
				}
			}
			finally
			{
				Semaphore.Release();
			}
		}

		public async Task AddStatuses( IList<Status> statuses )
		{
			await Semaphore.WaitAsync( SemaphoreWait );
			try
			{
				using( var tx = new Transaction( Connection ) )
				{
					int count = statuses.Count;
					const int batchSize = 100;
					int runsNeeded = (int)Math.Ceiling( count / (float)batchSize );

					for( int batchIdx = 0; batchIdx < runsNeeded; ++batchIdx )
					{
						var items = statuses.Skip( batchIdx * batchSize ).Take( batchSize );

						using( var cmd = Connection.CreateCommand() )
						{
							cmd.CommandText = "INSERT OR REPLACE INTO Statuses (Id, UserId, StatusData, Expires) VALUES ";

							cmd.CommandText += string.Join( ",", items.Select( ( s, i ) =>
							{
								// ReSharper disable AccessToDisposedClosure
								cmd.AddParameter( $"id{i}", s.GetStatusId() );
								cmd.AddParameter( $"userid{i}", s.User.GetUserId() );
								cmd.AddParameter( $"json{i}", JsonConvert.SerializeObject( s ) );

								// ReSharper restore AccessToDisposedClosure

								return $"( @id{i}, @userid{i}, @json{i}, @expires )";
							} ) );

							cmd.AddParameter( "expires", SqliteHelper.GetDateValue( DateTime.Now.Add( Constants.Cache.StatusExpiration ) ) );
							await cmd.ExecuteNonQueryAsync();
						}
					}

					tx.Commit();
				}
			}
			finally
			{
				Semaphore.Release();
			}
		}

		public async Task AddUsers( IList<UserCacheEntry> users )
		{
			int count = users.Count;
			const int batchSize = 100;
			int runsNeeded = (int)Math.Ceiling( count / (float)batchSize );
			await Semaphore.WaitAsync( SemaphoreWait );

			try
			{
				using( var tx = new Transaction( Connection ) )
				{
					for( int batchIdx = 0; batchIdx < runsNeeded; ++batchIdx )
					{
						var items = users.Skip( batchIdx * batchSize ).Take( batchSize );

						using( var cmd = Connection.CreateCommand() )
						{
							cmd.CommandText = "INSERT OR REPLACE INTO Users (Id, UserName, UserData, Expires) VALUES ";

							cmd.CommandText += string.Join( ",", items.Select( ( u, i ) =>
							{
								// ReSharper disable AccessToDisposedClosure
								cmd.AddParameter( $"userId{i}", u.UserId );
								cmd.AddParameter( $"userName{i}", u.UserName );
								cmd.AddParameter( $"json{i}", u.Data );

								// ReSharper restore AccessToDisposedClosure

								return $"( @userId{i}, @userName{i}, @json{i}, @expires )";
							} ) );

							cmd.AddParameter( "expires", SqliteHelper.GetDateValue( DateTime.Now.Add( Constants.Cache.UserInfoExpiration ) ) );
							await cmd.ExecuteNonQueryAsync();
						}
					}

					tx.Commit();
				}
			}
			finally
			{
				Semaphore.Release();
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

		public async Task<List<MessageCacheEntry>> GetMessages()
		{
			await Cleanup();

			List<MessageCacheEntry> result = new List<MessageCacheEntry>();

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT Id, Sender, Recipient, Data FROM Messages;";

				using( var reader = await cmd.ExecuteReaderAsync() )
				{
					while( await reader.ReadAsync() )
					{
						result.Add( await MessageCacheEntry.Read( reader ) );
					}
				}
			}

			return result;
		}

		public async Task<Status> GetStatus( ulong id )
		{
			await Cleanup();

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT StatusData FROM Statuses WHERE id = @id;";
				cmd.AddParameter( "id", id );

				var json = await cmd.ExecuteScalarAsync() as string;
				if( json == null )
				{
					return null;
				}

				return JsonConvert.DeserializeObject<Status>( json );
			}
		}

		public async Task<List<Status>> GetStatusesForColumn( Guid columnId )
		{
			List<Status> result = new List<Status>();

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText =
					"SELECT s.StatusData FROM ColumnStatuses c LEFT JOIN Statuses s ON c.StatusId = s.Id " +
					" WHERE c.ColumnId = @columnId ORDER BY s.Id";
				cmd.AddParameter( "columnId", columnId );

				using( var reader = await cmd.ExecuteReaderAsync() )
				{
					while( await reader.ReadAsync() )
					{
						var json = await reader.GetFieldValueAsync<string>( 0 );
						result.Add( JsonConvert.DeserializeObject<Status>( json ) );
					}
				}
			}

			return result;
		}

		public async Task<List<Status>> GetStatusesForUser( ulong userId )
		{
			await Cleanup();

			List<Status> result = new List<Status>();

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT StatusData FROM Statuses WHERE UserId = @userId;";
				cmd.AddParameter( "userId", userId );

				using( var reader = await cmd.ExecuteReaderAsync() )
				{
					while( await reader.ReadAsync() )
					{
						var json = await reader.GetFieldValueAsync<string>( 0 );
						result.Add( JsonConvert.DeserializeObject<Status>( json ) );
					}
				}
			}

			return result;
		}

		public async Task<User> GetUser( ulong userId )
		{
			await Cleanup();

			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT UserData FROM Users WHERE Id = @userId;";
				cmd.AddParameter( "userId", userId );

				var json = await cmd.ExecuteScalarAsync() as string;
				if( !string.IsNullOrEmpty( json ) )
				{
					return JsonConvert.DeserializeObject<User>( json );
				}
			}

			return null;
		}

		public async Task MapStatusesToColumn( IList<Status> statuses, Guid columnId )
		{
			await Semaphore.WaitAsync( SemaphoreWait );
			try
			{
				using( var tx = new Transaction( Connection ) )
				{
					int count = statuses.Count;
					const int batchSize = 100;
					int runsNeeded = (int)Math.Ceiling( count / (float)batchSize );

					for( int batchIdx = 0; batchIdx < runsNeeded; ++batchIdx )
					{
						var items = statuses.Skip( batchIdx * batchSize ).Take( batchSize );

						using( var cmd = Connection.CreateCommand() )
						{
							cmd.CommandText = "INSERT OR REPLACE INTO ColumnStatuses (ColumnId, StatusId) VALUES ";
							cmd.AddParameter( "columnId", columnId );

							cmd.CommandText += string.Join( ",", items.Select( ( s, i ) =>
							{
								// ReSharper disable AccessToDisposedClosure
								cmd.AddParameter( $"statusId{i}", s.GetStatusId() );

								// ReSharper restore AccessToDisposedClosure

								return $"( @columnId, @statusId{i} )";
							} ) );

							await cmd.ExecuteNonQueryAsync();
						}
					}

					tx.Commit();
				}
			}
			finally
			{
				Semaphore.Release();
			}
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

		public async Task RemoveStatus( ulong id )
		{
			await Semaphore.WaitAsync( SemaphoreWait );
			try
			{
				using( var cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "DELETE FROM Statuses WHERE Id = @id;";
					cmd.AddParameter( "id", id );
					await cmd.ExecuteNonQueryAsync();
				}
			}
			finally
			{
				Semaphore.Release();
			}
		}

		public async Task SaveTwitterConfig( LinqToTwitter.Configuration cfg )
		{
			await Semaphore.WaitAsync( SemaphoreWait );
			try
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
					cmd.AddParameter( "expires",
						SqliteHelper.GetDateValue( DateTime.Now.Add( Constants.Cache.TwitterConfigExpiration ) ) );

					await cmd.ExecuteNonQueryAsync();
				}
			}
			finally
			{
				Semaphore.Release();
			}
		}

		private readonly SQLiteConnection Connection;
		private readonly SemaphoreSlim Semaphore = new SemaphoreSlim( 1, 1 );
		private readonly TimeSpan SemaphoreWait = TimeSpan.FromSeconds( 5 );
	}
}