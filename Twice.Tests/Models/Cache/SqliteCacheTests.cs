using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Twice.Models.Cache;

namespace Twice.Tests.Models.Cache
{
	[TestClass, ExcludeFromCodeCoverage]
	public class SqliteCacheTests
	{
		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task AddingUserTwiceUpdatesData()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var user = DummyGenerator.CreateDummyUserEx();
				user.UserID = 123;
				user.ScreenName = "test";

				await cache.AddUsers( new[] {new UserCacheEntry( user )} );

				// Act
				user.ScreenName = "testi";
				await cache.AddUsers( new[] {new UserCacheEntry( user )} );

				// Assert
				var fromDb = ( await cache.GetKnownUsers() ).First();

				Assert.AreEqual( "testi", fromDb.UserName );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task CachedHastagsCanBeRetrieved()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO Hashtags (Tag) VALUES ('test'), ('abc');";
					cmd.ExecuteNonQuery();
				}

				// Act
				var tags = ( await cache.GetKnownHashtags() ).ToArray();

				// Assert
				CollectionAssert.AreEquivalent( new[] {"test", "abc"}, tags );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task CachedUsersCanBeRetrieved()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO Users (Id, UserName, UserData) VALUES (@id1, @name1, @data1), "
					                  + "(@id2, @name2, @data2);";

					cmd.AddParameter( "id1", 111 );
					cmd.AddParameter( "id2", 222 );
					cmd.AddParameter( "name1", "testi" );
					cmd.AddParameter( "name2", "testUser" );

					var u = DummyGenerator.CreateDummyUser();
					u.UserID = 111;
					u.ScreenName = "testi";
					u.CreatedAt = new DateTime( 1, 2, 3, 4, 5, 6 );
					cmd.AddParameter( "data1", JsonConvert.SerializeObject( u ) );

					u = DummyGenerator.CreateDummyUser();
					u.UserID = 222;
					u.ScreenName = "testUser";
					u.CreatedAt = new DateTime( 6, 5, 4, 3, 2, 1 );
					cmd.AddParameter( "data2", JsonConvert.SerializeObject( u ) );

					cmd.ExecuteNonQuery();
				}

				// Act
				var users = ( await cache.GetKnownUsers() ).ToArray();

				// Assert
				Assert.AreEqual( 2, users.Length );
				Assert.IsNotNull( users.SingleOrDefault( u => u.UserId == 111 ) );
				Assert.IsNotNull( users.SingleOrDefault( u => u.UserId == 222 ) );
				Assert.IsNotNull( users.SingleOrDefault( u => u.UserName == "testi" ) );
				Assert.IsNotNull( users.SingleOrDefault( u => u.UserName == "testUser" ) );

				var data = JsonConvert.DeserializeObject<User>( users.First( u => u.UserId == 111 ).Data );
				Assert.AreEqual( new DateTime( 1, 2, 3, 4, 5, 6 ), data.CreatedAt );

				data = JsonConvert.DeserializeObject<User>( users.First( u => u.UserId == 222 ).Data );
				Assert.AreEqual( new DateTime( 6, 5, 4, 3, 2, 1 ), data.CreatedAt );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task CachingEmptyHashtagListDoesNothing()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				// Act
				var ex = await ExceptionAssert.CatchAsync<Exception>( () => cache.AddHashtags( new List<string>() ) );

				// Assert
				Assert.IsNull( ex );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public void ConstructingFromConnectionStringWorks()
		{
			// Arrange
			var sb = new SQLiteConnectionStringBuilder
			{
				DataSource = ":memory:"
			};

			// Act
			var ex = ExceptionAssert.Catch<Exception>( () => new SqliteCache( sb.ToString() ) );

			// Assert
			Assert.IsNull( ex, ex?.ToString() );
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public void DatabaseTablesAreCreatedOnConstruction()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( new SqliteCache( con ) )
			{
				// Act
				List<string> tableNames = new List<string>();
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
					using( var reader = cmd.ExecuteReader() )
					{
						while( reader.Read() )
						{
							tableNames.Add( reader.GetString( 0 ) );
						}
					}
				}

				// Assert
				CollectionAssert.AreEquivalent( new[]
				{
					"Hashtags", "Users", "TwitterConfig",
					"Statuses", "ColumnStatuses", "Messages",
					"UserFriends"
				}, tableNames );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task CorrectUserIdIsFoundForFriend()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO UserFriends (UserId, FriendId) VALUES (1, 5), (2,4), (3,5);";
					cmd.ExecuteNonQuery();
				}

				// Act
				var noFriend = await cache.FindFriend( 15 );
				var single = await cache.FindFriend( 4 );
				var two = await cache.FindFriend( 5 );

				// Assert
				Assert.AreEqual( 0ul, noFriend );
				Assert.AreEqual( 2ul, single );
				Assert.IsTrue( two == 1ul || two == 3ul, two.ToString() );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public void DisposeClosesDatabaseConnection()
		{
			// Arrange
			var con = OpenConnection();
			bool disposed = false;
			con.Disposed += ( s, e ) => disposed = true;
			var cache = new SqliteCache( con );

			// Act
			cache.Dispose();

			// Assert
			Assert.IsTrue( disposed );
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task ExpiredConfigurationIsNotLoaded()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO TwitterConfig (Data, Expires) VALUES ('test', 100);";
					cmd.ExecuteNonQuery();
				}

				// Act
				var cfg = await cache.ReadTwitterConfig();

				// Assert
				Assert.IsNull( cfg );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task ExpiredHashtagIsNotRetrieved()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO Hashtags (Tag, Expires) VALUES ('test', 100);";
					cmd.ExecuteNonQuery();
				}

				// Act
				var tags = ( await cache.GetKnownHashtags() ).ToArray();

				// Assert
				Assert.AreEqual( 0, tags.Length );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task ExpiredUserIsNotRetrieved()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO Users (Id, UserName, UserData, Expires) VALUES (123, 'test', 'test', 100);";
					cmd.ExecuteNonQuery();
				}

				// Act
				var users = ( await cache.GetKnownUsers() ).ToArray();

				// Assert
				Assert.AreEqual( 0, users.Length );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task HashtagCanBeAdded()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				// Act
				await cache.AddHashtags( new[] {"test"} );

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT Tag FROM Hashtags WHERE Tag = 'test';";

					var fromDb = cmd.ExecuteScalar();
					Assert.AreEqual( "test", fromDb );
				}
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task MessageCanBeAdded()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var message = DummyGenerator.CreateDummyMessage();
				message.SenderID = 111;
				message.RecipientID = 222;
				message.ID = 333;
				message.Text = "Hello World";

				var entry = new MessageCacheEntry( message );

				// Act
				await cache.AddMessages( new[] {entry} );

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT Id, Sender, Recipient, Data FROM Messages;";
					using( var reader = cmd.ExecuteReader() )
					{
						Assert.IsTrue( reader.Read() );

						Assert.AreEqual( 333, reader.GetInt32( 0 ) );
						Assert.AreEqual( 111, reader.GetInt32( 1 ) );
						Assert.AreEqual( 222, reader.GetInt32( 2 ) );

						var json = reader.GetString( 3 );
						var from = JsonConvert.DeserializeObject<DirectMessage>( json );
						Assert.AreEqual( message.Text, from.Text );
					}
				}
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task MessageCanBeRetrieved()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var message = DummyGenerator.CreateDummyMessage();
				message.ID = 111;
				message.SenderID = 222;
				message.RecipientID = 333;
				message.Text = "Hello World";
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO Messages (Id, Sender, Recipient, Data) VALUES (111,222,333, @json);";
					cmd.AddParameter( "json", JsonConvert.SerializeObject( message ) );
					cmd.ExecuteNonQuery();
				}

				// Act
				var list = await cache.GetMessages();
				var fromDb = list.FirstOrDefault();

				// Assert
				Assert.IsNotNull( fromDb );

				Assert.AreEqual( message.SenderID, fromDb.Sender );
				Assert.AreEqual( message.RecipientID, fromDb.Recipient );
				Assert.AreEqual( message.ID, fromDb.Id );

				var msg = JsonConvert.DeserializeObject<DirectMessage>( fromDb.Data );
				Assert.AreEqual( message.Text, msg.Text );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task OldFriendsAreOverwritten()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO UserFriends (UserId, FriendId) VALUES (123,1), (123,2), (123,3);";
					cmd.ExecuteNonQuery();
				}

				// Act
				await cache.SetUserFriends( 123, new ulong[] {4, 5, 6} );

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT FriendId FROM UserFriends WHERE UserId = '123' ORDER BY FriendId;";
					using( var reader = cmd.ExecuteReader() )
					{
						Assert.IsTrue( reader.Read() );
						int id = reader.GetInt32( 0 );
						Assert.AreEqual( 4, id );

						Assert.IsTrue( reader.Read() );
						id = reader.GetInt32( 0 );
						Assert.AreEqual( 5, id );

						Assert.IsTrue( reader.Read() );
						id = reader.GetInt32( 0 );
						Assert.AreEqual( 6, id );
					}
				}
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task ReadingNonExistingConfigurationReturnsNull()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				// Act
				var cfg = await cache.ReadTwitterConfig();

				// Assert
				Assert.IsNull( cfg );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task StatusCanBeAdded()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var status = DummyGenerator.CreateDummyStatus();
				status.Text = "hello world";
				status.User.UserID = status.UserID = 222;
				status.ID = 111;

				// Act
				await cache.AddStatuses( new[] {status} );

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT Id, UserId, StatusData FROM Statuses";
					using( var reader = cmd.ExecuteReader() )
					{
						Assert.IsTrue( reader.Read() );

						Assert.AreEqual( 111, reader.GetInt32( 0 ) );
						Assert.AreEqual( 222, reader.GetInt32( 1 ) );

						var from = JsonConvert.DeserializeObject<Status>( reader.GetString( 2 ) );

						Assert.AreEqual( status.Text, from.Text );
					}
				}
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task StatusCanBeRemoved()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO Statuses (Id, UserId, StatusData) VALUES (123, 111, 'test');";
					cmd.ExecuteNonQuery();
				}

				// Act
				await cache.RemoveStatus( 123 );

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT Id FROM Statuses WHERE Id = 123;";
					var fromDb = cmd.ExecuteScalar();

					Assert.IsNull( fromDb );
				}
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task StatusCanBeRetrieved()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var status = DummyGenerator.CreateDummyStatus();
				status.Text = "hello world";
				status.UserID = 222;
				status.ID = 111;
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO Statuses (Id, UserId, StatusData) VALUES (111, 222, @json);";
					cmd.AddParameter( "json", JsonConvert.SerializeObject( status ) );
					cmd.ExecuteNonQuery();
				}

				// Act
				var fromDb = await cache.GetStatus( 111 );

				// Assert
				Assert.AreEqual( status.UserID, fromDb.UserID );
				Assert.AreEqual( status.Text, fromDb.Text );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task StatusesCanBeMappedToColumns()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var id1 = Guid.NewGuid();
				var id2 = Guid.NewGuid();

				var s1 = new List<Status>
				{
					DummyGenerator.CreateDummyStatus(),
					DummyGenerator.CreateDummyStatus()
				};

				s1[0].StatusID = 1;
				s1[1].StatusID = 2;

				var s2 = new List<Status>
				{
					DummyGenerator.CreateDummyStatus()
				};

				s2[0].StatusID = 3;

				// Act
				await cache.MapStatusesToColumn( s1, id1 );
				await cache.MapStatusesToColumn( s2, id2 );

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT StatusId FROM ColumnStatuses WHERE ColumnId=@col ORDER BY StatusId ASC;";
					cmd.AddParameter( "col", id1 );

					using( var reader = cmd.ExecuteReader() )
					{
						Assert.IsTrue( reader.Read() );
						Assert.AreEqual( 1, reader.GetInt32( 0 ) );

						Assert.IsTrue( reader.Read() );
						Assert.AreEqual( 2, reader.GetInt32( 0 ) );
					}
				}

				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT StatusId FROM ColumnStatuses WHERE ColumnId=@col ORDER BY StatusId ASC;";
					cmd.AddParameter( "col", id2 );

					using( var reader = cmd.ExecuteReader() )
					{
						Assert.IsTrue( reader.Read() );
						Assert.AreEqual( 3, reader.GetInt32( 0 ) );
					}
				}
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task StatusesForColumnCanBeLoaded()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var user = DummyGenerator.CreateDummyUser();
				user.UserID = 123;

				var s1 = DummyGenerator.CreateDummyStatus( user );
				s1.ID = 1;
				var s2 = DummyGenerator.CreateDummyStatus( user );
				s2.ID = 2;
				var s3 = DummyGenerator.CreateDummyStatus( user );
				s3.ID = 3;

				await cache.AddStatuses( new[] {s1, s2, s3} );

				var c1 = Guid.NewGuid();
				var c2 = Guid.NewGuid();
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO ColumnStatuses (ColumnId, StatusId) VALUES (@c1, 1), (@c1, 2), (@c2, 3);";
					cmd.AddParameter( "c1", c1 );
					cmd.AddParameter( "c2", c2 );
					cmd.ExecuteNonQuery();
				}

				// Act
				var statusesC1 = ( await cache.GetStatusesForColumn( c1 ) ).ToArray();
				var statusesC2 = ( await cache.GetStatusesForColumn( c2 ) ).ToArray();

				// Assert
				Assert.AreEqual( 2, statusesC1.Length );
				Assert.IsNotNull( statusesC1.SingleOrDefault( s => s.ID == 1 ) );
				Assert.IsNotNull( statusesC1.SingleOrDefault( s => s.ID == 2 ) );

				Assert.AreEqual( 1, statusesC2.Length );
				Assert.IsNotNull( statusesC2.SingleOrDefault( s => s.ID == 3 ) );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task StatusesForUserCanBeLoaded()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var user = DummyGenerator.CreateDummyUser();
				user.UserID = 123;

				var s1 = DummyGenerator.CreateDummyStatus( user );
				s1.ID = 1;
				var s2 = DummyGenerator.CreateDummyStatus( user );
				s2.ID = 2;
				var s3 = DummyGenerator.CreateDummyStatus( user );
				s3.ID = 3;

				await cache.AddStatuses( new[] {s1, s2, s3} );

				// Act
				var statuses = ( await cache.GetStatusesForUser( 123 ) ).ToArray();

				// Assert
				Assert.AreEqual( 3, statuses.Length );
				Assert.IsNotNull( statuses.SingleOrDefault( s => s.ID == 1 ) );
				Assert.IsNotNull( statuses.SingleOrDefault( s => s.ID == 2 ) );
				Assert.IsNotNull( statuses.SingleOrDefault( s => s.ID == 3 ) );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task TwitterConfigCanBeRetrieved()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var cfg = new LinqToTwitter.Configuration
				{
					PhotoSizeLimit = 123
				};

				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO TwitterConfig (Data) VALUES (@json);";
					var p = cmd.CreateParameter();
					p.ParameterName = "json";
					p.Value = JsonConvert.SerializeObject( cfg );
					cmd.Parameters.Add( p );

					cmd.ExecuteNonQuery();
				}

				// Act
				var loaded = await cache.ReadTwitterConfig();

				// Assert
				Assert.AreEqual( cfg.PhotoSizeLimit, loaded.PhotoSizeLimit );
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task TwitterConfigCanBeStored()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var cfg = new LinqToTwitter.Configuration
				{
					PhotoSizeLimit = 123
				};

				// Act
				await cache.SaveTwitterConfig( cfg );

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT Data FROM TwitterConfig";
					var fromDb = JsonConvert.DeserializeObject<LinqToTwitter.Configuration>( (string)cmd.ExecuteScalar() );

					Assert.AreEqual( cfg.PhotoSizeLimit, fromDb.PhotoSizeLimit );
				}
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task UserCanBeAdded()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				var user = DummyGenerator.CreateDummyUserEx();
				user.UserID = 123;
				user.ScreenName = "testi";

				var entry = new UserCacheEntry( user );

				// Act
				await cache.AddUsers( new[] {entry} );

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT Id, UserName, UserData FROM Users";
					using( var reader = cmd.ExecuteReader() )
					{
						Assert.IsTrue( reader.Read() );

						Assert.AreEqual( 123L, reader.GetInt64( 0 ) );
						Assert.AreEqual( "testi", reader.GetString( 1 ) );

						var jsonUser = JsonConvert.DeserializeObject<User>( reader.GetString( 2 ) );
						Assert.AreEqual( user.UserID, jsonUser.UserID );
						Assert.AreEqual( user.ScreenName, jsonUser.ScreenName );
					}
				}
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task UserFriendsCanBeCached()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				// Act
				await cache.SetUserFriends( 123, new ulong[] {1, 2, 3} );

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT FriendId FROM UserFriends WHERE UserId = '123' ORDER BY FriendId;";
					using( var reader = cmd.ExecuteReader() )
					{
						Assert.IsTrue( reader.Read() );
						int id = reader.GetInt32( 0 );
						Assert.AreEqual( 1, id );

						Assert.IsTrue( reader.Read() );
						id = reader.GetInt32( 0 );
						Assert.AreEqual( 2, id );

						Assert.IsTrue( reader.Read() );
						id = reader.GetInt32( 0 );
						Assert.AreEqual( 3, id );
					}
				}
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public async Task UserFriendsCanBeRead()
		{
			// Arrange
			using( var con = OpenConnection() )
			using( var cache = new SqliteCache( con ) )
			{
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO UserFriends (UserId, FriendId) VALUES (123,1), (123,2), (123,3);";
					cmd.ExecuteNonQuery();
				}

				// Act
				var friends = ( await cache.GetUserFriends( 123 ) ).ToArray();

				// Assert
				CollectionAssert.AreEquivalent( new ulong[] {1, 2, 3}, friends );
			}
		}

		private static SQLiteConnection OpenConnection()
		{
			var sb = new SQLiteConnectionStringBuilder
			{
				DataSource = ":memory:"
			};

			var connection = new SQLiteConnection( sb.ToString() );

			return connection.OpenAndReturn();
		}
	}
}