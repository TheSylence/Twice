using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using Twice.Models.Columns;

namespace Twice.Models.Twitter.Streaming
{
	internal class StreamingRepository : IStreamingRepository
	{
		public StreamingRepository( ITwitterContextList contextList )
		{
			ContextList = contextList;
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
					.Twitter.Streaming.Where( s => s.Type == StreamingType.User ) ) );

				LoadedParsers.Add( key, parser );
			}
			return parser;
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

		protected readonly Dictionary<ParserKey, IStreamParser> LoadedParsers = new Dictionary<ParserKey, IStreamParser>();
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