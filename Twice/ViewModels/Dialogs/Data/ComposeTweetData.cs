using System;
using Twice.ViewModels.Twitter;
using Twice.Views.Dialogs;

namespace Twice.ViewModels.Dialogs.Data
{
	internal class ComposeTweetData : DialogData
	{
		public ComposeTweetData( StatusViewModel status = null, bool replyToAll = false )
			: base( typeof( MessageDialog ), typeof( IComposeTweetViewModel ) )
		{
			Status = status;
			ReplyToAll = replyToAll;
		}

		public override bool Equals( DialogData obj )
		{
			throw new NotImplementedException();
		}

		public override object GetResult( object viewModel )
		{
			return null;
		}

		public override void Setup( object viewModel )
		{
			var vm = CastViewModel<IComposeTweetViewModel>( viewModel );
			vm.SetReply( Status, ReplyToAll );
		}

		private bool ReplyToAll;
		private StatusViewModel Status;
	}
}