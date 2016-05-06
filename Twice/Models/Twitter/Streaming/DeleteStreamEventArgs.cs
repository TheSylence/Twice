using LitJson;

namespace Twice.Models.Twitter.Streaming
{
	/// <summary>
	///     Arguments for a streaming event involding a deletion.
	/// </summary>
	internal class DeleteStreamEventArgs : StreamEventArgs
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="DeleteStreamEventArgs" /> class.
		/// </summary>
		/// <param name="json">The content.</param>
		public DeleteStreamEventArgs( string json )
			: base( json )
		{
			JsonData data = JsonMapper.ToObject( json )["delete"][0];
			Id = (ulong)data["id"];
			UserId = (ulong)data["user_id"];
		}

		/// <summary>
		///     ID of the deleted status.
		/// </summary>
		public ulong Id { get; private set; }

		/// <summary>
		///     ID of the user.
		/// </summary>
		public ulong UserId { get; private set; }
	}
}