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
}