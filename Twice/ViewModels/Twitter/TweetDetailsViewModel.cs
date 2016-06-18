using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Twitter
{
	[ConfigureAwait( false )]
	internal class TweetDetailsViewModel : DialogViewModel, ITweetDetailsViewModel
	{
		public TweetDetailsViewModel()
		{
			PreviousConversationTweets = new ObservableCollection<StatusViewModel>();
			FollowingConversationTweets = new ObservableCollection<StatusViewModel>();
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
				RaisePropertyChanged( nameof( PreviousConversationTweets ) );

				status = inReplyTo;
			}

			await Task.WhenAll( PreviousConversationTweets.Select( s => s.LoadQuotedTweet() ) );
			IsLoadingPrevious = false;
		}

		private async Task StartLoadingResponses()
		{
			IsLoadingFollowing = true;

			var replies = await Context.Twitter.Search.SearchReplies( DisplayTweet.Model );
			await Dispatcher.RunAsync( () =>
				FollowingConversationTweets.AddRange(
					replies.Select( r => new StatusViewModel( r, Context, Configuration, ViewServiceRepository ) ) ) );

			await Task.WhenAll( FollowingConversationTweets.Select( s => s.LoadQuotedTweet() ) );

			RaisePropertyChanged( nameof( FollowingConversationTweets ) );
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
					DisplayTweet.LoadQuotedTweet() );
		}

		public IContextEntry Context { get; set; }

		public StatusViewModel DisplayTweet
		{
			[DebuggerStepThrough] get { return _DisplayTweet; }
			set
			{
				if( _DisplayTweet == value )
				{
					return;
				}

				_DisplayTweet = value;
				RaisePropertyChanged();
			}
		}

		public IList<StatusViewModel> FollowingConversationTweets { get; }

		public bool IsLoadingFollowing
		{
			[DebuggerStepThrough] get { return _IsLoadingFollowing; }
			private set
			{
				if( _IsLoadingFollowing == value )
				{
					return;
				}

				_IsLoadingFollowing = value;
				RaisePropertyChanged();
			}
		}

		public bool IsLoadingPrevious
		{
			[DebuggerStepThrough] get { return _IsLoadingPrevious; }
			private set
			{
				if( _IsLoadingPrevious == value )
				{
					return;
				}

				_IsLoadingPrevious = value;
				RaisePropertyChanged();
			}
		}

		public IList<StatusViewModel> PreviousConversationTweets { get; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private StatusViewModel _DisplayTweet;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsLoadingFollowing;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsLoadingPrevious;
	}
}