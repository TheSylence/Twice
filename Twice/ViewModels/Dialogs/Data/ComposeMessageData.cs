using Twice.ViewModels.Twitter;
using Twice.Views.Dialogs;

namespace Twice.ViewModels.Dialogs.Data
{
	internal class ComposeMessageData : DialogData
	{
		public ComposeMessageData( string screenName = null, MessageViewModel message = null )
			: base( typeof( MessageDialog ), typeof( IComposeMessageViewModel ) )
		{
			ScreenName = screenName;
			Message = message;
		}

		public override bool Equals( DialogData obj )
		{
			var other = obj as ComposeMessageData;
			if( other == null )
			{
				return false;
			}

			return ScreenName?.Equals( other.ScreenName ) == true
				&& Message?.Equals( other.Message ) == true;
		}

		public override object GetResult( object viewModel )
		{
			return null;
		}

		public override void Setup( object viewModel )
		{
			var vm = CastViewModel<IComposeMessageViewModel>( viewModel );

			vm.Recipient = ScreenName;
			vm.InReplyTo = Message;
		}

		private MessageViewModel Message;
		private string ScreenName;
	}
}