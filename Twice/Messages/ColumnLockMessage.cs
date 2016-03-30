using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Messaging;

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