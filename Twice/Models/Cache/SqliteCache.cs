﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
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

		public async Task AddStatus( Status status )
		{
			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "INSERT OR REPLACE INTO Statuses (Id, UserId, StatusData, Expires) "
								+ "VALUES (@id, @userid, @json, @expires);";

				cmd.AddParameter( "id", status.GetStatusId() );
				cmd.AddParameter( "userid", status.User.GetUserId() );
				cmd.AddParameter( "json", JsonConvert.SerializeObject( status ) );
				cmd.AddParameter( "expires", SqliteHelper.GetDateValue( DateTime.Now.Add( StatusExpiration ) ) );

				await cmd.ExecuteNonQueryAsync();
			}
		}

		public async Task AddStatuses( IList<Status> statuses )
		{
			using( var tx = Connection.BeginTransaction() )
			{
				int count = statuses.Count;
				const int batchSize = 100;
				int runsNeeded = (int)Math.Ceiling( count / (float)batchSize );
				List<Task> tasks = new List<Task>( runsNeeded );

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

						cmd.AddParameter( "expires", SqliteHelper.GetDateValue( DateTime.Now.Add( StatusExpiration ) ) );
						tasks.Add( cmd.ExecuteNonQueryAsync() );
					}
				}

				await Task.WhenAll( tasks );
				tx.Commit();
			}
		}

		public async Task AddUser( UserCacheEntry user )
		{
			using( var cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "INSERT OR REPLACE INTO Users (Id, UserName, UserData, Expires) VALUES " +
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
					"SELECT s.StatusData FROM ColumnStatuses c LEFT JOIN Statuses s ON c.StatusId = s.Id "+
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

		public async Task MapStatusesToColumn( IList<Status> statuses, Guid columnId )
		{
			using( var tx = Connection.BeginTransaction() )
			{
				int count = statuses.Count;
				const int batchSize = 100;
				int runsNeeded = (int)Math.Ceiling( count / (float)batchSize );
				List<Task> tasks = new List<Task>( runsNeeded );
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

						tasks.Add( cmd.ExecuteNonQueryAsync() );
					}
				}

				await Task.WhenAll( tasks );
				tx.Commit();
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
		private readonly TimeSpan StatusExpiration = TimeSpan.FromDays( 20 );
		private readonly TimeSpan TwitterConfigExpiration = TimeSpan.FromDays( 1 );
		private readonly TimeSpan UserExpiration = TimeSpan.FromDays( 14 );
	}
}