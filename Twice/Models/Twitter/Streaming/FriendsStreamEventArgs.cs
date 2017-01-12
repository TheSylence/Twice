using LitJson;
using System.Linq;

namespace Twice.Models.Twitter.Streaming
{
	/// <summary>
	///  Arguments for a streaming event containing friends of the authorizing user. 
	/// </summary>
	internal class FriendsStreamEventArgs : StreamEventArgs
	{
		/// <summary>
		///  Initializes a new instance of the <see cref="FriendsStreamEventArgs" /> class. 
		/// </summary>
		/// <param name="json"> The json encoded data. </param>
		public FriendsStreamEventArgs( string json )
			: base( json )
		{
			JsonData parsed = JsonMapper.ToObject( json )["friends"];
			Friends = parsed.Cast<JsonData>().Select( j => (ulong)j ).ToArray();
		}

		/// <summary>
		///  IDs of the user's friends. 
		/// </summary>
		public ulong[] Friends { get; private set; }
	}
}