using GalaSoft.MvvmLight.Messaging;
using System.Diagnostics.CodeAnalysis;

namespace Twice.Messages
{
	/// <summary>
	///     Message that is sent when dragging occured for a resize thumb.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal class DragMessage : MessageBase
	{
		public DragMessage( object sender, bool start )
			: base( sender )
		{
			Start = start;
		}

		/// <summary>
		///     Indicates whether this message indicates a drag start or a drag complete.
		/// </summary>
		public bool Start { get; private set; }
	}
}