using LinqToTwitter;
using LitJson;

namespace Twice.Models.Twitter.Streaming
{
	/// <summary>
	/// Arguments for a streaming event associated to a status.
	/// </summary>
	internal class StatusStreamEventArgs : StreamEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StatusStreamEventArgs"/> class.
		/// </summary>
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

		/// <summary>
		/// The received status.
		/// </summary>
		public Status Status { get; private set; }
	}
}