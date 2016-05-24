using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using LinqToTwitter;
using Ninject;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Services.Views;

namespace Twice.ViewModels.Twitter
{
	internal class TweetDetailsViewModel : DialogViewModel, ITweetDetailsViewModel
	{
		public TweetDetailsViewModel()
		{
			PreviousConversationTweets = new ObservableCollection<StatusViewModel>();
			FollowingConversationTweets = new ObservableCollection<StatusViewModel>();
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

			IsLoadingPrevious = true;
			await LoadPreviousTweets( DisplayTweet.Model );
			IsLoadingPrevious = false;

			IsLoadingFollowing = true;
			await LoadFollowingTweets( DisplayTweet.Model );
			IsLoadingFollowing = false;
		}

		private Task LoadFollowingTweets( Status status )
		{
			// TODO: Implement
			return Task.CompletedTask;
		}

		private async Task LoadPreviousTweets( Status status )
		{
			if( status.InReplyToStatusID == 0 )
			{
				return;
			}

			var inReplyTo = await Context.Twitter.Statuses.GetTweet( status.InReplyToStatusID, true );
			if( inReplyTo == null )
			{
				return;
			}

			var vm = new StatusViewModel( inReplyTo, Context, Config, ViewServices );

			await Dispatcher.RunAsync( () => PreviousConversationTweets.Insert( 0, vm ) );

			await LoadPreviousTweets( inReplyTo );
		}

		[Inject]
		public IConfig Config { get; set; }

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
			set
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
			set
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

		[Inject]
		public IViewServiceRepository ViewServices { get; set; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private StatusViewModel _DisplayTweet;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsLoadingFollowing;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsLoadingPrevious;
	}
}