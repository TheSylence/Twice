using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Anotar.NLog;
using Fody;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Ninject;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Entities;
using Twice.Resources;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Profile
{
	[ConfigureAwait( false )]
	internal class ProfileDialogViewModel : DialogViewModel, IProfileDialogViewModel
	{
		internal void OverwriteContext( IContextEntry context )
		{
			Context = context;
		}

		private async void ExecuteFollowUserCommand()
		{
			try
			{
				await Context.Twitter.Users.FollowUser( User.UserId );
			}
			catch( Exception ex )
			{
				Notifier.DisplayMessage( ex.GetReason(), NotificationType.Error );
				return;
			}

			if( Friendship?.TargetRelationship?.FollowedBy == null )
			{
				return;
			}

			Friendship.TargetRelationship.FollowedBy = true;
			RaisePropertyChanged( nameof(Friendship) );
		}

		private async void ExecuteUnfollowUserCommand()
		{
			try
			{
				await Context.Twitter.Users.UnfollowUser( User.UserId );
			}
			catch( Exception ex )
			{
				Notifier.DisplayMessage( ex.GetReason(), NotificationType.Error );
				return;
			}

			if( Friendship?.TargetRelationship?.FollowedBy != null )
			{
				Friendship.TargetRelationship.FollowedBy = false;
				RaisePropertyChanged( nameof(Friendship) );
			}
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

			if( maxId == null )
			{
				await Dispatcher.RunAsync( () => Center() );
			}

			if( statuses.Any() )
			{
				MaxId = Math.Min( MaxId, statuses.Min( s => s.Id ) );

				Task.WhenAll( statuses.Select( s => s.LoadDataAsync() ) ).ContinueWith( async t =>
				{
					if( maxId == null )
					{
						await Dispatcher.RunAsync( () => Center() );
					}
				} ).Forget();
			}
			return statuses;
		}

		private async Task<IEnumerable<object>> LoadStatuses()
		{
			return await LoadStatuses( null );
		}

		public async Task OnLoad( object data )
		{
			if( ProfileId == 0 && string.IsNullOrWhiteSpace( ScreenName ) )
			{
				Close( false );
				return;
			}

			IsBusy = true;
			var contextId = await ( Cache?.FindFriend( ProfileId ) ?? Task.FromResult( 0ul ) );

			IContextEntry friendContext = ContextList.Contexts.FirstOrDefault( ctx => ctx.UserId == contextId );
			Context = friendContext ?? ContextList.Contexts.FirstOrDefault( ctx => ctx.IsDefault ) ?? ContextList.Contexts.First();

			UserEx user = null;
			try
			{
				if( ProfileId == 0 )
				{
					user = await Context.Twitter.Users.ShowUser( ScreenName, true );
				}
				else
				{
					user = await Context.Twitter.Users.ShowUser( ProfileId, true );
				}
			}
			catch( Exception ex )
			{
				LogTo.WarnException( "Failed to load user profile", ex );
				Notifier.DisplayMessage( ex.GetReason(), NotificationType.Error );
			}
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
				new UserSubPage( Strings.Tweets, LoadStatuses, LoadMoreStatuses, User.Model.StatusesCount )
				{
					Dispatcher = Dispatcher
				},
				new UserSubPage( Strings.Following, LoadFollowings, User.Model.FriendsCount )
				{
					Dispatcher = Dispatcher
				},
				new UserSubPage( Strings.Followers, LoadFollowers, User.Model.FollowersCount )
				{
					Dispatcher = Dispatcher
				}
			};
			RaisePropertyChanged( nameof(UserPages) );

			await Dispatcher.RunAsync( () => Center() );
			IsBusy = false;
		}

		public ICommand FollowUserCommand => _FollowUserCommand ?? ( _FollowUserCommand = new RelayCommand(
			                                     ExecuteFollowUserCommand ) );

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

		public void Setup( ulong profileId )
		{
			ProfileId = profileId;
		}

		public void Setup( string screenName )
		{
			ScreenName = screenName;
		}

		public ICommand UnfollowUserCommand => _UnfollowUserCommand ?? ( _UnfollowUserCommand = new RelayCommand(
			                                       ExecuteUnfollowUserCommand ) );

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

				Title = string.Format( Strings.ProfileOfUser, User?.Model?.ScreenNameResponse );
				RaisePropertyChanged( nameof(Title) );
			}
		}

		public ICollection<UserSubPage> UserPages { get; private set; }

		[Inject]
		public INotifier Notifier { get; set; }

		private RelayCommand _FollowUserCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private Friendship _Friendship;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsBusy;

		private RelayCommand _UnfollowUserCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private UserViewModel _User;

		private IContextEntry Context;
		private ulong MaxId = ulong.MaxValue;
		private ulong ProfileId;
		private string ScreenName;
	}
}