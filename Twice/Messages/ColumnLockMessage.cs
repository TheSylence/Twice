using GalaSoft.MvvmLight.Messaging;
using System.Diagnostics.CodeAnalysis;

namespace Twice.Messages
{
	[ExcludeFromCodeCoverage]
	internal class ColumnLockMessage : MessageBase
	{
		public ColumnLockMessage( bool locked )
		{
			Locked = locked;
		}

		public readonly bool Locked;
	}
}