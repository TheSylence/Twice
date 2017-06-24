using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.Resources;

namespace Twice.ViewModels.Twitter
{
	[ConfigureAwait( false )]
	internal class TweetDetailsViewModel : DialogViewModel, ITweetDetailsViewModel
	{
		public TweetDetailsViewModel()
		{
			PreviousConversationTweets = new ObservableCollection<StatusViewModel>();
			FollowingConversationTweets = new ObservableCollection<StatusViewModel>();
			Title = Strings.Tweet;
		}

		private async Task StartLoadingPrevTweets()
		{
			Status status = DisplayTweet.Model;

			IsLoadingPrevious = true;
			while( true )
			{
				if( status.InReplyToStatusID == 0 )
				{
					break;
				}

				var inReplyTo = await Context.Twitter.Statuses.GetTweet( status.InReplyToStatusID, true );
				if( inReplyTo == null )
				{
					break;
				}

				var vm = new StatusViewModel( inReplyTo, Context, Configuration, ViewServiceRepository );

				await Dispatcher.RunAsync( () => PreviousConversationTweets.Insert( 0, vm ) );
				RaisePropertyChanged( nameof(PreviousConversationTweets) );

				status = inReplyTo;
			}

			await Task.WhenAll( PreviousConversationTweets.Select( s => s.LoadDataAsync() ) );
			IsLoadingPrevious = false;

			await Dispatcher.RunAsync( () => ScrollRequested?.Invoke( this, EventArgs.Empty ) );
		}

		private async Task StartLoadingResponses()
		{
			IsLoadingFollowing = true;

			var replies = await Context.Twitter.Search.SearchReplies( DisplayTweet.Model );
			await Dispatcher.RunAsync( () =>
				FollowingConversationTweets.AddRange(
					replies.Select( r => new StatusViewModel( r, Context, Configuration, ViewServiceRepository ) ) ) );

			await Task.WhenAll( FollowingConversationTweets.Select( s => s.LoadDataAsync() ) );

			RaisePropertyChanged( nameof(FollowingConversationTweets) );
			IsLoadingFollowing = false;
		}

		private async Task StartLoadingRetweets()
		{
			await DisplayTweet.LoadRetweets();
		}

		public async Task OnLoad( object data )
		{
			if( DisplayTweet == null )
			{
				Close( false );
				return;
			}

			PreviousConversationTweets.Clear();
			FollowingConversationTweets.Clear();

			await
				Task.WhenAll( StartLoadingPrevTweets(), StartLoadingResponses(), StartLoadingRetweets(),
					DisplayTweet.LoadDataAsync() ).ContinueWith( async t => { await Dispatcher.RunAsync( Center ); } );
		}

		public event EventHandler ScrollRequested;

		public IContextEntry Context { get; set; }

		public StatusViewModel DisplayTweet { get; set; }

		public IList<StatusViewModel> FollowingConversationTweets { get; }

		public bool IsLoadingFollowing { get; set; }

		public bool IsLoadingPrevious { get; set; }

		public IList<StatusViewModel> PreviousConversationTweets { get; }
	}
}