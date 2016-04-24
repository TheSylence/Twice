using GalaSoft.MvvmLight.Messaging;

namespace Twice.Messages
{
	internal class WaitMessage : MessageBase
	{
		public WaitMessage( bool start )
		{
			Start = start;
		}

		public bool Start { get; }
	}
}