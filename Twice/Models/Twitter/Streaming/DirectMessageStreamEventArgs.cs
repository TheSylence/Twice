using LinqToTwitter;
using LitJson;

namespace Twice.Models.Twitter.Streaming
{
	/// <summary>
	///     Arguments for a streaming event associated with a direct message.
	/// </summary>
	internal class DirectMessageStreamEventArgs : StreamEventArgs
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="DirectMessageStreamEventArgs" /> class.
		/// </summary>
		/// <param name="json">The json encoded data.</param>
		public DirectMessageStreamEventArgs( string json )
			: base( json )
		{
			var obj = JsonMapper.ToObject( json );
			JsonData messageData;
			if( obj.TryGetValue( "direct_message", out messageData ) )
			{
				Message = new DirectMessage( messageData );
			}
			else
			{
				Message = new DirectMessage();
			}
		}

		/// <summary>
		///     The associated message.
		/// </summary>
		public DirectMessage Message { get; private set; }
	}
}