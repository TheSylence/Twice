using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.Utilities;
using Twice.Utilities.Os;

namespace Twice.ViewModels.Twitter
{
	internal class StatusViewModel : ObservableObject
	{
		public StatusViewModel( Status model, IContextEntry context )
		{
			Context = context;

			Model = model;
			OriginalStatus = Model;
			if( OriginalStatus.RetweetedStatus != null && OriginalStatus.RetweetedStatus.StatusID != 0 )
			{
				Model = OriginalStatus.RetweetedStatus;
				SourceUser = new UserViewModel( OriginalStatus.User );
			}
			else
			{
				Model = OriginalStatus;
				SourceUser = null;
			}

			User = new UserViewModel( Model.User );
		}

		private bool CanExecuteBlockUserCommand()
		{
			return OriginalStatus.User.GetUserId() != Context.UserId;
		}

		private bool CanExecuteDeleteStatusCommand()
		{
			return OriginalStatus.User.GetUserId() == Context.UserId;
		}

		private bool CanExecuteReplyToAllCommand()
		{
			List<ulong> userIds = new List<ulong>
			{
				OriginalStatus.User.GetUserId(),
				Model.User.GetUserId()
			};
			userIds.AddRange( Model.Entities.UserMentionEntities.Select( m => m.Id ) );

			return userIds.Distinct().Count() > 1;
		}

		private bool CanExecuteReportSpamCommand()
		{
			return OriginalStatus.User.GetUserId() != Context.UserId;
		}

		private bool CanExecuteRetweetStatusCommand()
		{
			return OriginalStatus.User.GetUserId() != Context.UserId;
		}

		private void ExecAsync( Action action, string message = null, NotificationType type = NotificationType.Information )
		{
			IsLoading = true;
			Task.Run( action ).ContinueWith( t =>
			{
				DispatcherHelper.CheckBeginInvokeOnUI( () => IsLoading = false );
			} ).ContinueWith( t =>
			{
				if( !string.IsNullOrWhiteSpace( message ) )
				{
					Context.Notifier.DisplayMessage( message, type );
				}
			} );
		}

		private void ExecuteBlockUserCommand()
		{
			ExecAsync( async () => await Context.Twitter.CreateBlockAsync( OriginalStatus.UserID, null, true ), Strings.BlockedUser, NotificationType.Success );
		}

		private void ExecuteCopyTweetCommand()
		{
			string text = $"@{Model.User.GetScreenName()}: {Model.Text}";

			Clipboard.SetText( text );
		}

		private void ExecuteCopyTweetUrlCommand()
		{
			Clipboard.SetText( Model.GetUrl().AbsoluteUri );
		}

		private void ExecuteDeleteStatusCommand()
		{
			ExecAsync( async () => await Context.Twitter.DeleteTweetAsync( OriginalStatus.StatusID ), Strings.StatusDeleted, NotificationType.Success );
		}

		private void ExecuteFavoriteStatusCommand()
		{
			ExecAsync( async () =>
			{
				if( !Model.Favorited )
				{
					await Context.Twitter.CreateFavoriteAsync( Model.StatusID );
				}
				else
				{
					await Context.Twitter.DestroyFavoriteAsync( Model.StatusID );
				}

				Model.Favorited = !Model.Favorited;
				RaisePropertyChanged( nameof( IsFavorited ) );
			}, Model.Favorited ? Strings.RemovedFavorite : Strings.AddedFavorite, NotificationType.Success );
		}

		private void ExecuteReplyCommand()
		{
		}

		private void ExecuteReplyToAllCommand()
		{
		}

		private void ExecuteReportSpamCommand()
		{
		}

		private void ExecuteRetweetStatusCommand()
		{
			ExecAsync( async () =>
			{
				await Context.Twitter.RetweetAsync( Model.StatusID );
				Model.Retweeted = true;
				RaisePropertyChanged( nameof( IsRetweeted ) );
			}, Strings.RetweetedStatus );
		}

		public ICommand BlockUserCommand => _BlockUserCommand ?? ( _BlockUserCommand = new RelayCommand( ExecuteBlockUserCommand, CanExecuteBlockUserCommand ) );

		public IClipboard Clipboard
		{
			get { return _Clipboard ?? DefaultClipboard; }
			set { _Clipboard = value; }
		}

		public ICommand CopyTweetCommand => _CopyTweetCommand ?? ( _CopyTweetCommand = new RelayCommand( ExecuteCopyTweetCommand ) );
		public ICommand CopyTweetUrlCommand => _CopyTweetUrlCommand ?? ( _CopyTweetUrlCommand = new RelayCommand( ExecuteCopyTweetUrlCommand ) );
		public DateTime CreatedAt => Model.CreatedAt;
		public ICommand DeleteStatusCommand => _DeleteStatusCommand ?? ( _DeleteStatusCommand = new RelayCommand( ExecuteDeleteStatusCommand, CanExecuteDeleteStatusCommand ) );
		public bool DisplayMedia => InlineMedias.Any();
		public ICommand FavoriteStatusCommand => _FavoriteStatusCommand ?? ( _FavoriteStatusCommand = new RelayCommand( ExecuteFavoriteStatusCommand ) );
		public ulong Id => Model.StatusID;

		public IEnumerable<Uri> InlineMedias
		{
			get
			{
				if( _InlineMedias == null )
				{
					var entities = Model.Entities.MediaEntities.Select( m => new Uri( m.MediaUrlHttps ) );
					//var custom = Services.GetService<IMediaService>().ExtractMedias( Model );

					//_InlineMedias = new List<Uri>( entities.Concat( custom ) );
					_InlineMedias = new List<Uri>( entities );
				}

				return _InlineMedias;
			}
		}

		public bool IsFavorited => Model.Favorited;

		public bool IsLoading
		{
			[DebuggerStepThrough]
			get
			{
				return _IsLoading;
			}
			set
			{
				if( _IsLoading == value )
				{
					return;
				}

				_IsLoading = value;
				RaisePropertyChanged();
			}
		}

		public bool IsRetweeted => Model.Retweeted;
		public Status Model { get; }
		public ICommand ReplyCommand => _ReplyCommand ?? ( _ReplyCommand = new RelayCommand( ExecuteReplyCommand ) );
		public ICommand ReplyToAllCommand => _ReplyToAllCommand ?? ( _ReplyToAllCommand = new RelayCommand( ExecuteReplyToAllCommand, CanExecuteReplyToAllCommand ) );
		public ICommand ReportSpamCommand => _ReportSpamCommand ?? ( _ReportSpamCommand = new RelayCommand( ExecuteReportSpamCommand, CanExecuteReportSpamCommand ) );
		public ICommand RetweetStatusCommand => _RetweetStatusCommand ?? ( _RetweetStatusCommand = new RelayCommand( ExecuteRetweetStatusCommand, CanExecuteRetweetStatusCommand ) );
		public UserViewModel SourceUser { get; }
		public UserViewModel User { get; }
		private static readonly IClipboard DefaultClipboard = new ClipboardWrapper();
		private readonly IContextEntry Context;
		private readonly Status OriginalStatus;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _BlockUserCommand;

		private IClipboard _Clipboard;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _CopyTweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _CopyTweetUrlCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _DeleteStatusCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _FavoriteStatusCommand;

		private List<Uri> _InlineMedias;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsLoading;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _ReplyCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _ReplyToAllCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _ReportSpamCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _RetweetStatusCommand;
	}
}