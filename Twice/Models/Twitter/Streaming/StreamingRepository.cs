using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using Seal.Fody;
using Twice.Models.Columns;

namespace Twice.Models.Twitter.Streaming
{
	[LeaveUnsealed]
	internal class StreamingRepository : IStreamingRepository
	{
		public StreamingRepository( ITwitterContextList contextList )
		{
			ContextList = contextList;
		}

		private void Dispose( bool disposing )
		{
			if( disposing )
			{
				foreach( var kvp in LoadedParsers )
				{
					kvp.Value.Dispose();
				}

				LoadedParsers.Clear();
			}
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
				parser =
					StreamParser.Create( new StreamingConnection( ContextList.Contexts.First( c => c.UserId == userId )
						.Twitter.Streaming.GetUserStream() ) );

				LoadedParsers.Add( key, parser );
			}
			return parser;
		}

		private readonly ITwitterContextList ContextList;
		protected readonly Dictionary<ParserKey, IStreamParser> LoadedParsers = new Dictionary<ParserKey, IStreamParser>();

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