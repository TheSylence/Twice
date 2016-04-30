using LinqToTwitter;
using LitJson;
using System;
using System.Globalization;

namespace Twice.Models.Twitter.Streaming
{
	/// <summary>
	/// Arguments for a streaming event associated with a non-tweet event.
	/// </summary>
	internal class EventStreamEventArgs : StreamEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EventStreamEventArgs"/> class.
		/// </summary>
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

		/// <summary>
		/// Date and time this event has been created.
		/// </summary>
		public DateTime CreatedAt { get; private set; }

		/// <summary>
		/// Type of associated event.
		/// </summary>
		public StreamEventType Event { get; private set; }

		/// <summary>
		/// Raw event type.
		/// </summary>
		public string EventStr { get; private set; }

		/// <summary>
		/// The source user.
		/// </summary>
		public User Source { get; private set; }

		/// <summary>
		/// The target user.
		/// </summary>
		public User Target { get; private set; }
	}
}