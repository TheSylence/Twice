using System;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Columns
{
	class MessageEventArgs : EventArgs
	{
		public MessageEventArgs( MessageViewModel message )
		{
			Message = message;
		}

		public readonly MessageViewModel Message;
	}
}