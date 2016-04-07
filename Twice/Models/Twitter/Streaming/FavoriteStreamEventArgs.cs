using LinqToTwitter;
using LitJson;

namespace Twice.Models.Twitter.Streaming
{
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
}