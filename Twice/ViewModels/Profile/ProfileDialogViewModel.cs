using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fody;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Profile
{
	[ConfigureAwait( false )]
	internal class ProfileDialogViewModel : DialogViewModel, IProfileDialogViewModel
	{
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

		private async Task<IEnumerable<object>> LoadStatuses()
		{
			//IEnumerable<Status> cached = Services.GetService<IStatusCache>().GetStatusesOfUser( ProfileID );
			IEnumerable<Status> cached = Enumerable.Empty<Status>().ToArray();
			IEnumerable<Status> newStatuses;

			if( cached.Any() )
			{
				ulong since = cached.Max( c => c.StatusID );

				newStatuses = await Context.Twitter.Statuses.GetUserTweets( User.UserId, since );
			}
			else
			{
				newStatuses = await Context.Twitter.Statuses.GetUserTweets( User.UserId );
			}

			return
				cached.Concat( newStatuses ).OrderByDescending( s => s.StatusID ).Select(
					s => new StatusViewModel( s, Context, Configuration, ViewServiceRepository ) );
		}

		public async Task OnLoad( object data )
		{
			if( ProfileId == 0 )
			{
				Debugger.Break();
				return;
			}

			IsBusy = true;
			Context = ContextList.Contexts.First();

			var user = await Context.Twitter.Users.ShowUser( ProfileId, true );
			if( user == null )
			{
				// TODO: Handle errors
				return;
			}

			User = new UserViewModel( user );
			Friendship = await Context.Twitter.Friendships.GetFriendshipWith( Context.UserId, User.UserId );

			UserPages = new List<UserSubPage>
			{
				new UserSubPage( Strings.Tweets, LoadStatuses, User.Model.StatusesCount ),
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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private Friendship _Friendship;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsBusy;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private UserViewModel _User;

		private IContextEntry Context;
		private ulong ProfileId;
	}
}