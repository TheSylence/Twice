using Twice.ViewModels.Twitter;
using Twice.Views.Dialogs;

namespace Twice.ViewModels.Dialogs.Data
{
	internal class MessageData : DialogData
	{
		public MessageData( MessageViewModel message )
			: base( typeof( MessageDetailsDialog ), typeof( IMessageDetailsViewModel ) )
		{
			Message = message;
		}

		public override bool Equals( DialogData obj )
		{
			var other = obj as MessageData;
			return Message.Equals( other?.Message );
		}

		public override object GetResult( object viewModel )
		{
			return null;
		}

		public override void Setup( object viewModel )
		{
			var vm = CastViewModel<IMessageDetailsViewModel>( viewModel );
			vm.Message = Message;
		}

		private MessageViewModel Message;
	}
}