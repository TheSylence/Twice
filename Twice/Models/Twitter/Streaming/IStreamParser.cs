using System;

namespace Twice.Models.Twitter.Streaming
{
	internal interface IStreamParser : IDisposable
	{
		/// <summary>Occurs when a status was deleted.</summary>
		event EventHandler<DeleteStreamEventArgs> DirectMessageDeleted;

		/// <summary>Occurs when a direct message was received.</summary>
		event EventHandler<DirectMessageStreamEventArgs> DirectMessageReceived;

		/// <summary>Occurs when a status was favourited.</summary>
		event EventHandler<FavoriteStreamEventArgs> FavoriteEventReceived;

		/// <summary>Occurs when the friend list was received.</summary>
		event EventHandler<FriendsStreamEventArgs> FriendsReceived;

		/// <summary>Occurs when a status was deleted.</summary>
		event EventHandler<DeleteStreamEventArgs> StatusDeleted;

		/// <summary>Occurs when a status was received.</summary>
		event EventHandler<StatusStreamEventArgs> StatusReceived;

		/// <summary>Occurs when unknown data has been received.</summary>
		event EventHandler<StreamEventArgs> UnknownDataReceived;

		/// <summary>Occurs when an event was received.</summary>
		event EventHandler<EventStreamEventArgs> UnknownEventReceived;

		void StartStreaming();
	}
}