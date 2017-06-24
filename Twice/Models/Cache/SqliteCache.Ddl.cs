using System.Collections.Generic;

namespace Twice.Models.Cache
{
	internal partial class SqliteCache
	{
		private IEnumerable<string> GetDdlQueries()
		{
			yield return "CREATE TABLE IF NOT EXISTS Hashtags ( Id INT PRIAMRY KEY, Tag TEXT NOT NULL, Expires BIGINT );";

			yield return
				"CREATE TABLE IF NOT EXISTS Users ( Id INT PRIMARY KEY, UserName TEXT NOT NULL, UserData TEXT, Expires BIGINT );";

			yield return
				"CREATE TABLE IF NOT EXISTS Statuses ( Id INT PRIMARY KEY, UserId INT NOT NULL, StatusData TEXT NOT NULL, Expires BIGINT );"
				;

			yield return "CREATE TABLE IF NOT EXISTS TwitterConfig ( Data TEXT, Expires BIG INT );";

			yield return
				"CREATE TABLE IF NOT EXISTS ColumnStatuses ( StatusId INT, ColumnId GUID, PRIMARY KEY( StatusId, ColumnId ) );";

			yield return "CREATE TABLE IF NOT EXISTS Messages ( Id INT PRIMARY KEY, Sender INT, Recipient INT, Data TEXT, Expires BIG INT );";

			yield return "CREATE TABLE IF NOT EXISTS UserFriends( UserId BIGINT, FriendId BIGINT );";
		}

		private IEnumerable<string> GetInitQueries()
		{
			yield return "PRAGMA synchronous = off;";
		}
	}
}