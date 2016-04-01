using LinqToTwitter;
using LitJson;

namespace Twice.Models.Twitter
{
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
}