using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.Models.Cache;

namespace Twice.Tests.Models.Cache
{
	[ExcludeFromCodeCoverage, TestClass]
	public class TransactionTests
	{
		[TestMethod, TestCategory( "Models.Cache" )]
		public void TransactionCanBeCommited()
		{
			// Arrange
			using( var con = OpenConnection() )
			{
				var tx = new Transaction( con );

				// Act
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO t (c) VALUES (1);";
					cmd.ExecuteNonQuery();
				}

				tx.Commit();

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT c FROM t;";
					var fromDb = cmd.ExecuteScalar();

					Assert.AreEqual( "1", fromDb.ToString() );
				}
			}
		}

		[TestMethod, TestCategory( "Models.Cache" )]
		public void TransactionRollsBackWithoutCommit()
		{
			// Arrange
			using( var con = OpenConnection() )
			{
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO t (c) VALUES (1);";
					cmd.ExecuteNonQuery();
				}

				var tx = new Transaction( con );

				// Act
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "UPDATE t SET c = 2;";
					cmd.ExecuteNonQuery();
				}

				tx.Dispose();

				// Assert
				using( var cmd = con.CreateCommand() )
				{
					cmd.CommandText = "SELECT c FROM t;";
					var fromDb = cmd.ExecuteScalar();

					Assert.AreEqual( "1", fromDb.ToString() );
				}
			}
		}

		private static SQLiteConnection OpenConnection()
		{
			var sb = new SQLiteConnectionStringBuilder
			{
				DataSource = ":memory:"
			};

			var connection = new SQLiteConnection( sb.ToString() );
			connection.Open();

			using( var cmd = connection.CreateCommand() )
			{
				cmd.CommandText = "CREATE TABLE t (c INTEGER);";
				cmd.ExecuteNonQuery();
			}

			return connection;
		}
	}
}