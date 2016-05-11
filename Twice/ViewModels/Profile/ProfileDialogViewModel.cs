using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Ninject;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Profile
{
	[ConfigureAwait( false )]
	internal class ProfileDialogViewModel : DialogViewModel, IProfileDialogViewModel
	{
		[Inject]
		public INotifier Notifier { get; set; }

		public async Task OnLoad( object data )
		{
			if( ProfileId == 0 )
			{
				Close( false );
				return;
			}

			IsBusy = true;
			Context = ContextList.Contexts.First();

			var user = await Context.Twitter.Users.ShowUser( ProfileId, true );
			if( user == null )
			{
				Notifier.DisplayMessage( Strings.UserNotFound, NotificationType.Error );
				Close( false );
				return;
			}

			User = new UserViewModel( user );
			Friendship = await Context.Twitter.Friendships.GetFriendshipWith( Context.UserId, User.UserId );

			UserPages = new List<UserSubPage>
			{
				new UserSubPage( Strings.Tweets, LoadStatuses, LoadMoreStatuses, User.Model.StatusesCount ),
				new UserSubPage( Strings.Following, LoadFollowings, User.Model.FriendsCount ),
				new UserSubPage( Strings.Followers, LoadFollowers, User.Model.FollowersCount )
			};
			RaisePropertyChanged( nameof( UserPages ) );

			IsBusy = false;
		}

		public void Setup( ulong profileId )
		{
			ProfileId = profileId;
		}

		private async Task<IEnumerable<object>> LoadFollowers()
		{
			var users = await Context.Twitter.Friendships.ListFollowers( User.UserId );

			return users.Select( u => new UserViewModel( u ) );
		}

		private async Task<IEnumerable<object>> LoadFollowings()
		{
			var users = await Context.Twitter.Friendships.ListFriends( User.UserId );

			return users.Select( u => new UserViewModel( u ) );
		}

		private async Task<IEnumerable<object>> LoadMoreStatuses()
		{
			return await LoadStatuses( MaxId );
		}

		private async Task<IEnumerable<object>> LoadStatuses( ulong? maxId )
		{
			var newStatuses = await Context.Twitter.Statuses.GetUserTweets( User.UserId, 0, maxId ?? 0 );

			var statuses = newStatuses.OrderByDescending( s => s.StatusID ).Select(
				s => new StatusViewModel( s, Context, Configuration, ViewServiceRepository ) ).ToArray();

			if( statuses.Any() )
			{
				MaxId = Math.Min( MaxId, statuses.Min( s => s.Id ) );
			}
			return statuses;
		}

		private async Task<IEnumerable<object>> LoadStatuses()
		{
			return await LoadStatuses( null );
		}

		public Friendship Friendship
		{
			[DebuggerStepThrough] get { return _Friendship; }
			set
			{
				if( _Friendship == value )
				{
					return;
				}

				_Friendship = value;
				RaisePropertyChanged();
			}
		}

		public bool IsBusy
		{
			[DebuggerStepThrough] get { return _IsBusy; }
			set
			{
				if( _IsBusy == value )
				{
					return;
				}

				_IsBusy = value;
				RaisePropertyChanged();
			}
		}

		public UserViewModel User
		{
			[DebuggerStepThrough] get { return _User; }
			set
			{
				if( _User == value )
				{
					return;
				}

				_User = value;
				RaisePropertyChanged();
			}
		}

		public ICollection<UserSubPage> UserPages { get; private set; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private Friendship _Friendship;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsBusy;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private UserViewModel _User;

		private IContextEntry Context;
		private ulong MaxId = ulong.MaxValue;
		private ulong ProfileId;
	}
}