using Anotar.NLog;
using LinqToTwitter;
using Seal.Fody;
using System;
using System.Collections.Generic;
using System.Linq;
using Twice.Models.Cache;
using Twice.Models.Columns;
using Twice.Models.Twitter.Entities;

namespace Twice.Models.Twitter.Streaming
{
	[LeaveUnsealed]
	internal class StreamingRepository : IStreamingRepository
	{
		// ReSharper disable once MemberCanBeProtected.Global
		public StreamingRepository( ITwitterContextList contextList, ICache cache )
		{
			ContextList = contextList;
			Cache = cache;
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		public IStreamParser GetParser( ColumnDefinition column )
		{
			var userId = column.SourceAccounts.First();

			var key = new ParserKey( userId, StreamingType.User );
			IStreamParser parser;

			if( !LoadedParsers.TryGetValue( key, out parser ) )
			{
				var context = ContextList.Contexts.First( c => c.UserId == userId );

				parser =
					StreamParser.Create( new StreamingConnection( context.Twitter.Streaming.GetUserStream() ), context );

				AddParser( parser, key );
			}
			return parser;
		}

		protected void AddParser( IStreamParser parser, ParserKey key )
		{
			parser.FriendsReceived += Parser_FriendsReceived;
			LoadedParsers.Add( key, parser );
		}

		private void Dispose( bool disposing )
		{
			if( !disposing )
			{
				return;
			}

			foreach( var kvp in LoadedParsers )
			{
				kvp.Value.Dispose();
			}

			LoadedParsers.Clear();
		}

		private async void Parser_FriendsReceived( object sender, FriendsStreamEventArgs e )
		{
			var parser = sender as IStreamParser;
			if( parser == null )
			{
				return;
			}

			var context = parser.AssociatedContext;

			var completeList = e.Friends.ToList();
			LogTo.Info( $"Received {completeList.Count} of user's friends" );
			var usersToAdd = new List<UserEx>( completeList.Count );

			await Cache.SetUserFriends( context.UserId, completeList );

			while( completeList.Any() )
			{
				var userList = string.Join( ",", completeList.Take( 100 ) );
				completeList.RemoveRange( 0, Math.Min( 100, completeList.Count ) );

				var userData = await context.Twitter.Users.LookupUsers( userList );
				usersToAdd.AddRange( userData );
			}

			if( usersToAdd.Any() )
			{
				//Debug.Assert( usersToAdd.Count == e.Friends.Length );
				await Cache.AddUsers( usersToAdd.Select( u => new UserCacheEntry( u ) ).ToList() );
			}
		}

		protected readonly Dictionary<ParserKey, IStreamParser> LoadedParsers = new Dictionary<ParserKey, IStreamParser>();
		private readonly ICache Cache;
		private readonly ITwitterContextList ContextList;

		protected class ParserKey
		{
			public ParserKey( ulong id, StreamingType type )
			{
				UserId = id;
				StreamingType = type;
			}

			public override bool Equals( object obj )
			{
				var other = obj as ParserKey;
				if( other == null )
				{
					return false;
				}

				return other.UserId == UserId && other.StreamingType == StreamingType;
			}

			public override int GetHashCode()
			{
				int hash = 23;
				unchecked
				{
					hash = hash * 17 + UserId.GetHashCode();
					hash = hash * 17 + StreamingType.GetHashCode();
				}
				return hash;
			}

			public StreamingType StreamingType { get; }
			public ulong UserId { get; }
		}
	}
}