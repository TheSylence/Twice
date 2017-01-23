using Twice.ViewModels.Twitter;
using Twice.Views.Dialogs;

namespace Twice.ViewModels.Dialogs.Data
{
	internal class QuoteTweetData : DialogData
	{
		public QuoteTweetData( StatusViewModel status, ulong[] preselectedAccounts )
			: base( typeof( TweetComposer ), typeof( IComposeTweetViewModel ) )
		{
			Status = status;
			PreSelectedAccounts = preselectedAccounts;
		}

		public override bool Equals( DialogData obj )
		{
			var other = obj as QuoteTweetData;
			if( other == null )
			{
				return false;
			}

			return Status.Equals( other.Status )
				&& PreSelectedAccounts.Compare( other.PreSelectedAccounts );
		}

		public override object GetResult( object viewModel )
		{
			return null;
		}

		public override void Setup( object viewModel )
		{
			var vm = CastViewModel<IComposeTweetViewModel>( viewModel );

			vm.QuotedTweet = Status;
			vm.PreSelectAccounts( PreSelectedAccounts );
		}

		private ulong[] PreSelectedAccounts;
		private StatusViewModel Status;
	}
}