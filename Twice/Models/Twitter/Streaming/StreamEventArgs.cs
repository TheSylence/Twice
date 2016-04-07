using System;

namespace Twice.Models.Twitter.Streaming
{
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