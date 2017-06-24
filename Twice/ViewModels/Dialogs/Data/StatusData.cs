using Twice.ViewModels.Twitter;
using Twice.Views.Dialogs;

namespace Twice.ViewModels.Dialogs.Data
{
	internal class StatusData : DialogData
	{
		public StatusData( StatusViewModel status )
			: base( typeof( TweetDetailsDialog ), typeof( ITweetDetailsViewModel ) )
		{
			Status = status;
		}

		public override bool Equals( DialogData obj )
		{
			var other = obj as StatusData;
			return Status.Equals( other?.Status );
		}

		public override object GetResult( object viewModel )
		{
			return null;
		}

		public override void Setup( object viewModel )
		{
			var vm = CastViewModel<ITweetDetailsViewModel>( viewModel );

			vm.Context = Status.Context;
			vm.DisplayTweet = Status;
		}

		private readonly StatusViewModel Status;
	}
}