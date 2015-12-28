using LinqToTwitter;
using LitJson;
using System;
using System.Globalization;
using System.Linq;

namespace Twice.Models.Twitter
{
	/// <summary>Enumeration for streaming events.</summary>
	internal enum StreamEventType
	{
		/// <summary>Unknown event.</summary>
		Unknown,

		/// <summary>User follows someone or User is followed.</summary>
		Follow,

		/// <summary>User updates their profile or User updates their protected status.</summary>
		UserUpdate,

		/// <summary>User favorites a Tweet or User’s Tweet is favorited.</summary>
		Favorite,

		/// <summary>User unfavorites a Tweet or User’s Tweet is unfavorited.</summary>
		Unfavorite,

		/// <summary>User blocks someone.</summary>
		Block,

		/// <summary>User removes a block.</summary>
		Unblock,

		/// <summary>User creates a list.</summary>
		ListCreated,

		/// <summary>User deletes a list.</summary>
		ListDestroyed,

		/// <summary>User edits a list.</summary>
		ListUpdated,

		/// <summary>User adds someone to a list or User is added to a list.</summary>
		ListMemberAdded,

		/// <summary>User removes someone from a list or User is removed from a list.</summary>
		ListMemberRemoved,

		/// <summary>User subscribes to a list or User’s list is subscribed to.</summary>
		ListUserSubscribed,

		/// <summary>User unsubscribes from a list or User’s list is unsubscribed from.</summary>
		ListUserUnsubscribed
	}

	/// <summary>Arguments for a streaming event involding a deletion.</summary>
	internal class DeleteStreamEventArgs : StreamEventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="DeleteStreamEventArgs"/> class.</summary>
		/// <param name="json">The content.</param>
		public DeleteStreamEventArgs( string json )
			: base( json )
		{
			JsonData data = JsonMapper.ToObject( json )["delete"][0];
			Id = (ulong)data["id"];
			UserId = (ulong)data["user_id"];
		}

		/// <summary>ID of the deleted status.</summary>
		public ulong Id { get; private set; }

		/// <summary>ID of the user.</summary>
		public ulong UserId { get; private set; }
	}

	/// <summary>Arguments for a streaming event associated with a direct message.</summary>
	internal class DirectMessageStreamEventArgs : StreamEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DirectMessageStreamEventArgs"/> class.
		/// </summary>
		/// <param name="json">The json encoded data.</param>
		public DirectMessageStreamEventArgs( string json )
			: base( json )
		{
			Message = new DirectMessage( JsonMapper.ToObject( json ) );
		}

		/// <summary>The associated message.</summary>
		public DirectMessage Message { get; private set; }
	}

	/// <summary>Arguments for a streaming event associated with a non-tweet event.</summary>
	internal class EventStreamEventArgs : StreamEventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="EventStreamEventArgs"/> class.</summary>
		/// <param name="json">The json encoded data.</param>
		public EventStreamEventArgs( string json )
			: base( json )
		{
			JsonData parsed = JsonMapper.ToObject( json );

			CreatedAt = DateTime.ParseExact( (string)parsed["created_at"], "ddd MMM dd HH:mm:ss %zzzz yyyy", CultureInfo.InvariantCulture );
			Source = new User( parsed["source"] );
			Target = new User( parsed["target"] );

			EventStr = (string)parsed["event"];
			Event = StreamEventType.Unknown;
			if( !string.IsNullOrWhiteSpace( EventStr ) )
			{
				try
				{
					Event = (StreamEventType)Enum.Parse( typeof( StreamEventType ), EventStr.Replace( "_", string.Empty ), true );
				}
				catch( ArgumentException )
				{
					Event = StreamEventType.Unknown;
				}
			}
		}

		/// <summary>Date and time this event has been created.</summary>
		public DateTime CreatedAt { get; private set; }

		/// <summary>Type of associated event.</summary>
		public StreamEventType Event { get; private set; }

		/// <summary>Raw event type.</summary>
		public string EventStr { get; private set; }

		/// <summary>The source user.</summary>
		public User Source { get; private set; }

		/// <summary>The target user.</summary>
		public User Target { get; private set; }
	}

	/// <summary>Arugments for a streaming event involing a favorite.</summary>
	internal class FavoriteStreamEventArgs : EventStreamEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FavoriteStreamEventArgs"/> class.
		/// </summary>
		/// <param name="json">The json encoded data.</param>
		public FavoriteStreamEventArgs( string json )
			: base( json )
		{
			JsonData parsed = JsonMapper.ToObject( json );
			JsonData targetObject = parsed["target_object"];
			if( targetObject != null )
			{
				JsonData tmp;
				if( targetObject.TryGetValue( "text", out tmp ) && !targetObject.TryGetValue( "sender", out tmp ) )
				{
					TargetStatus = new Status( targetObject );
				}
			}
		}

		/// <summary>The target status.</summary>
		public Status TargetStatus { get; private set; }
	}

	/// <summary>Arguments for a streaming event containing friends of the authorizing user.</summary>
	internal class FriendsStreamEventArgs : StreamEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FriendsStreamEventArgs"/> class.
		/// </summary>
		/// <param name="json">The json encoded data.</param>
		public FriendsStreamEventArgs( string json )
			: base( json )
		{
			JsonData parsed = JsonMapper.ToObject( json )["friends"];
			Friends = parsed.Cast<JsonData>().Select( j => (ulong)j ).ToArray();
		}

		/// <summary>IDs of the user's friends.</summary>
		public ulong[] Friends { get; private set; }
	}

	/// <summary>Arguments for a streaming event associated to a status.</summary>
	internal class StatusStreamEventArgs : StreamEventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="StatusStreamEventArgs"/> class.</summary>
		/// <param name="json">The content.</param>
		public StatusStreamEventArgs( string json )
			: base( json )
		{
			Status = new Status( JsonMapper.ToObject( json ) );
		}

		internal StatusStreamEventArgs( Status status )
			: base( string.Empty )
		{
			Status = status;
		}

		/// <summary>The received status.</summary>
		public Status Status { get; private set; }
	}

	/// <summary>Arguments for events that are send by the Stream parser.</summary>
	internal class StreamEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="StreamEventArgs"/> class.</summary>
		/// <param name="json">The json encoded data.</param>
		public StreamEventArgs( string json )
		{
			Json = json;
		}

		/// <summary>The content.</summary>
		public string Json { get; private set; }
	}
}