using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;

namespace CacheViewer
{
	public class HashtagEntry
	{
		public DateTime Expires { get; set; }
		public string Hashtag { get; set; }
		public ulong Id { get; set; }
	}

	public partial class MainWindow
	{
		public MainWindow()
		{
			Statuses = new ObservableCollection<StatusEntry>();
			Hashtags = new ObservableCollection<HashtagEntry>();
			Users = new ObservableCollection<UserEntry>();

			DataContext = this;
			InitializeComponent();

			Loaded += MainWindow_Loaded;
		}

		private DateTime Date( long n )
		{
			return EpochStart.Add( TimeSpan.FromSeconds( n ) );
		}

		private void MainWindow_Loaded( object sender, RoutedEventArgs e )
		{
			var sb = new SQLiteConnectionStringBuilder
			{
				DataSource = "cache.db3"
			};

			using( var connection = new SQLiteConnection( sb.ToString() ) )
			{
				connection.Open();

				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT Id, Tag, Expires FROM Hashtags;";
					using( var reader = cmd.ExecuteReader() )
					{
						while( reader.Read() )
						{
							var tag = new HashtagEntry();

							//var tmp = reader.GetValue( 0 );
							//tag.Id = (ulong)reader.GetInt32( 0 );
							tag.Hashtag = reader.GetString( 1 );
							tag.Expires = Date( reader.GetInt64( 2 ) );

							Hashtags.Add( tag );
						}
					}
				}

				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT Id, UserName, UserData, Expires FROM Users;";
					using( var reader = cmd.ExecuteReader() )
					{
						while( reader.Read() )
						{
							var usr = new UserEntry();
							usr.Id = (ulong)reader.GetInt64( 0 );
							usr.Name = reader.GetString( 1 );
							usr.Json = !reader.IsDBNull( 2 )
								? reader.GetString( 2 )
								: "";
							usr.Expires = Date( reader.GetInt64( 3 ) );

							Users.Add( usr );
						}
					}
				}

				using( var cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT Id, UserId, StatusData, Expires FROM Statuses;";
					using( var reader = cmd.ExecuteReader() )
					{
						while( reader.Read() )
						{
							var status = new StatusEntry();
							status.Id = (ulong)reader.GetInt64( 0 );
							status.User = (ulong)reader.GetInt64( 1 );
							status.Json = reader.GetString( 2 );
							status.Expires = Date( reader.GetInt64( 3 ) );

							Statuses.Add( status );
						}
					}
				}
			}
		}

		public ObservableCollection<HashtagEntry> Hashtags { get; }
		public ObservableCollection<StatusEntry> Statuses { get; }
		public ObservableCollection<UserEntry> Users { get; }
		private static readonly DateTime EpochStart = new DateTime( 1970, 1, 1, 0, 0, 0 );
	}

	public class StatusEntry
	{
		public DateTime Expires { get; set; }
		public ulong Id { get; set; }
		public string Json { get; set; }
		public ulong User { get; set; }
	}

	public class UserEntry
	{
		public DateTime Expires { get; set; }
		public ulong Id { get; set; }
		public string Json { get; set; }
		public string Name { get; set; }
	}
}