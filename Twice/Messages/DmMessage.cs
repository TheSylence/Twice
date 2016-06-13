using GalaSoft.MvvmLight.Messaging;
using LinqToTwitter;

namespace Twice.Messages
{
	internal class DmMessage : MessageBase
	{
		public DmMessage( DirectMessage directMessage, EntityAction action )
		{
			DirectMessage = directMessage;
			Action = action;
		}

		public EntityAction Action { get; }
		public DirectMessage DirectMessage { get; }
	}
}