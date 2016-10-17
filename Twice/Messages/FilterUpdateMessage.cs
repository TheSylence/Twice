using GalaSoft.MvvmLight.Messaging;

namespace Twice.Messages
{
	internal class FilterUpdateMessage : MessageBase
	{
		public int RemoveCount { get; set; }
	}
}