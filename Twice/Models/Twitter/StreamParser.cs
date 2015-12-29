using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LitJson;

namespace Twice.Models.Twitter
{
	/// <summary>Parser for twitter streams.</summary>
	internal class StreamParser : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="StreamParser"/> class.</summary>
		/// <param name="stream">The user stream.</param>
		private StreamParser( IQueryable<Streaming> stream )
		{
			Stream = stream;
		}

		/// <summary>Occurs when a status was deleted.</summary>
		public event EventHandler<DeleteStreamEventArgs> DirectMessageDeleted;

		/// <summary>Occurs when a direct message was received.</summary>
		public event EventHandler<DirectMessageStreamEventArgs> DirectMessageReceived;

		/// <summary>Occurs when a status was favourited.</summary>
		public event EventHandler<FavoriteStreamEventArgs> FavoriteEventReceived;

		/// <summary>Occurs when the friend list was received.</summary>
		public event EventHandler<FriendsStreamEventArgs> FriendsReceived;

		/// <summary>Occurs when a status was deleted.</summary>
		public event EventHandler<DeleteStreamEventArgs> StatusDeleted;

		/// <summary>Occurs when a status was received.</summary>
		public event EventHandler<StatusStreamEventArgs> StatusReceived;

		/// <summary>Occurs when unknown data has been received.</summary>
		public event EventHandler<StreamEventArgs> UnknownDataReceived;

		/// <summary>Occurs when an event was received.</summary>
		public event EventHandler<EventStreamEventArgs> UnknownEventReceived;

		/// <summary>Creates a new parser for the specified stream.</summary>
		/// <param name="userStream">The stream.</param>
		/// <returns>The created parser.</returns>
		public static StreamParser Create( IQueryable<Streaming> userStream )
		{
			return new StreamParser( userStream );
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting
		/// unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		public void StartStreaming()
		{
			Stream.StartAsync( c => Task.Run( () => ParseContent( c ) ) )
				.ContinueWith( t => Connections = t.Result );
		}

		/// <summary>Releases unmanaged and - optionally - managed resources.</summary>
		/// <param name="disposing">
		/// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
		/// only unmanaged resources.
		/// </param>
		protected void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( Connections != null )
				{
					foreach( Streaming s in Connections )
					{
						s.CloseStream();
					}
				}
			}
		}

		/// <summary>Handles a twitter event.</summary>
		/// <param name="json">The json describing the event.</param>
		private void HandleEvent( string json )
		{
			JsonData parsed = JsonMapper.ToObject( json );

			string eventStr = (string)parsed["event"];
			StreamEventType eventType = StreamEventType.Unknown;

			if( !string.IsNullOrWhiteSpace( eventStr ) )
			{
				try
				{
					eventType = (StreamEventType)Enum.Parse( typeof( StreamEventType ), eventStr.Replace( "_", string.Empty ), true );
				}
				catch( ArgumentException )
				{
					eventType = StreamEventType.Unknown;
				}
			}

			switch( eventType )
			{
			case StreamEventType.Favorite:
				var favhandler = FavoriteEventReceived;
				favhandler?.Invoke( this, new FavoriteStreamEventArgs( json ) );
				break;

			default:
				var handler = UnknownEventReceived;
				handler?.Invoke( this, new EventStreamEventArgs( json ) );
				break;
			}
		}

		/// <summary>Parses the content and raises events.</summary>
		/// <param name="content">The content.</param>
		private void ParseContent( IStreamContent content )
		{
			if( string.IsNullOrEmpty( content.Content ) )
			{
				return;
			}

			JsonData parsed = JsonMapper.ToObject( content.Content );
			JsonData parsedContent;

			// Was this a tweet?
			if( parsed.TryGetValue( "text", out parsedContent ) )
			{
				var handler = StatusReceived;
				handler?.Invoke( this, new StatusStreamEventArgs( content.Content ) );
			}
			// Or a direct message?
			else if( parsed.TryGetValue( "direct_message", out parsedContent ) )
			{
				var handler = DirectMessageReceived;
				handler?.Invoke( this, new DirectMessageStreamEventArgs( content.Content ) );
			}
			// Or has something been deleted?
			else if( parsed.TryGetValue( "delete", out parsedContent ) )
			{
				JsonData deleted;
				if( parsedContent.TryGetValue( "status", out deleted ) )
				{
					var handler = StatusDeleted;
					handler?.Invoke( this, new DeleteStreamEventArgs( content.Content ) );
				}
				else
				{
					var handler = DirectMessageDeleted;
					handler?.Invoke( this, new DeleteStreamEventArgs( content.Content ) );
				}
			}
			// Or a different event?
			else if( parsed.TryGetValue( "event", out parsedContent ) )
			{
				HandleEvent( content.Content );
			}
			// Is this the friend list of the user?
			else if( parsed.TryGetValue( "friends", out parsedContent ) )
			{
				var handler = FriendsReceived;
				handler?.Invoke( this, new FriendsStreamEventArgs( content.Content ) );
			}
			else
			{
				var handler = UnknownDataReceived;
				handler?.Invoke( this, new StreamEventArgs( content.Content ) );
			}
		}

		private readonly IQueryable<Streaming> Stream;
		private List<Streaming> Connections;
	}
}