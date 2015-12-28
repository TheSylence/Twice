using GalaSoft.MvvmLight.Messaging;

namespace Twice.Messages
{
	internal class ColumnLockMessage : MessageBase
	{
		public ColumnLockMessage( bool locked )
		{
			Locked = locked;
		}

		public readonly bool Locked;
	}
}