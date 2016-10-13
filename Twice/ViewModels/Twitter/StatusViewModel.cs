using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Anotar.NLog;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Models.Configuration;
using Twice.Models.Media;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Comparers;
using Twice.Resources;
using Twice.Utilities;
using Twice.Utilities.Os;
using Twice.Utilities.Ui;
using Twice.Views.Services;

namespace Twice.ViewModels.Twitter
{
	internal class StatusViewModel : ColumnItem
	{
		public StatusViewModel( Status model, IContextEntry context, IConfig config, IViewServiceRepository viewServiceRepo )
		{
			Config = config;
			Context = context;
			ViewServiceRepository = viewServiceRepo;

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

			HasSensibleContent = config?.General?.FilterSensitiveTweets == true && Model.PossiblySensitive;
			User = new UserViewModel( Model.User );
			Dispatcher = new DispatcherHelperWrapper();
			RetweetedBy = new SmartCollection<UserViewModel>();
		}

		public ulong ExtractQuotedTweetUrl()
		{
			var quoteUrl = Model?.Entities?.UrlEntities?.FirstOrDefault( e => TwitterHelper.IsTweetUrl( e.ExpandedUrl ) );
			return quoteUrl == null
				? 0
				: TwitterHelper.ExtractTweetId( quoteUrl.ExpandedUrl );
		}

		public async Task LoadDataAsync()
		{
			// ReSharper disable once UnusedVariable
			var dontWait = LoadCard();
			await Task.WhenAll( LoadQuotedTweet(), LoadInlineMedias() );
		}

		public async Task LoadRetweets()
		{
			var ids = await Context.Twitter.Statuses.FindRetweeters( Model.GetStatusId(), Constants.Gui.MaxRetweets );
			var retweeters = await Context.Twitter.Users.LookupUsers( ids );
			var users = retweeters.Select( rt => new UserViewModel( rt ) );
			
			RetweetedBy.Clear();
			RetweetedBy.AddRange( users );
		}

		public void RetweetStatus( ITwitterContext context )
		{
			ExecAsync( async () =>
			{
				await context.Statuses.RetweetAsync( Model.GetStatusId() );

				Model.Retweeted = true;
				RaisePropertyChanged( nameof( IsRetweeted ) );
			}, Strings.RetweetedStatus, NotificationType.Success );
		}

		private static void EnsureEntitiesAreNotNull( Entities ent )
		{
			if( ent.HashTagEntities == null )
			{
				ent.HashTagEntities = new List<HashTagEntity>();
			}
			if( ent.MediaEntities == null )
			{
				ent.MediaEntities = new List<MediaEntity>();
			}
			if( ent.SymbolEntities == null )
			{
				ent.SymbolEntities = new List<SymbolEntity>();
			}
			if( ent.UrlEntities == null )
			{
				ent.UrlEntities = new List<UrlEntity>();
			}
			if( ent.UserMentionEntities == null )
			{
				ent.UserMentionEntities = new List<UserMentionEntity>();
			}
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
			userIds.AddRange( Model.Entities?.UserMentionEntities?.Select( m => m.Id ) ?? Enumerable.Empty<ulong>() );

			return userIds.Any();
		}

		private bool CanExecuteReportSpamCommand()
		{
			return OriginalStatus.User.GetUserId() != Context.UserId;
		}

		private void ExecAsync( Func<Task> action, string message = null, NotificationType type = NotificationType.Information )
		{
			IsLoading = true;
			Task.Run( async () =>
			{
				try
				{
					await action();
					return null;
				}
				catch( TwitterQueryException ex )
				{
					LogTo.WarnException( "Failed to retweet status", ex );
					return ex.Message;
				}
			} ).ContinueWith( t =>
			{
				Dispatcher.CheckBeginInvokeOnUI( () => IsLoading = false );
				var err = t.Result;
				if( !string.IsNullOrWhiteSpace( err ) )
				{
					Context.Notifier.DisplayMessage( err, NotificationType.Error );
				}
				else if( !string.IsNullOrWhiteSpace( message ) )
				{
					Context.Notifier.DisplayMessage( message, type );
				}
			} );
		}

		private void ExecuteBlockUserCommand()
		{
			ExecAsync( async () => await Context.Twitter.CreateBlockAsync( OriginalStatus.UserID, null, true ),
				Strings.BlockedUser, NotificationType.Success );
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

		private async void ExecuteDeleteStatusCommand()
		{
			var csa = new ConfirmServiceArgs( Strings.ConfirmDeleteStatus );
			if( !await ViewServiceRepository.Confirm( csa ) )
			{
				return;
			}

			ExecAsync( async () => await Context.Twitter.Statuses.DeleteTweetAsync( OriginalStatus.StatusID ),
				Strings.StatusDeleted,
				NotificationType.Success );
		}

		private void ExecuteFavoriteStatusCommand()
		{
			ExecAsync( async () =>
			{
				if( !Model.Favorited )
				{
					await Context.Twitter.Favorites.CreateFavoriteAsync( Model.StatusID );
				}
				else
				{
					await Context.Twitter.Favorites.DestroyFavoriteAsync( Model.StatusID );
				}

				Model.Favorited = !Model.Favorited;
				RaisePropertyChanged( nameof( IsFavorited ) );
			}, Model.Favorited
				? Strings.RemovedFavorite
				: Strings.AddedFavorite, NotificationType.Success );
		}

		private async void ExecuteQuoteStatusCommand()
		{
			await ViewServiceRepository.QuoteTweet( this );
		}

		private async void ExecuteReplyCommand()
		{
			await ViewServiceRepository.ReplyToTweet( this, false );
		}

		private async void ExecuteReplyToAllCommand()
		{
			await ViewServiceRepository.ReplyToTweet( this, true );
		}

		private void ExecuteReportSpamCommand()
		{
			ExecAsync( async () => { await Context.Twitter.ReportAsSpam( Model.User.GetUserId() ); }, Strings.TweetReportedAsSpam );
		}

		private async void ExecuteRetweetStatusCommand()
		{
			await ViewServiceRepository.RetweetStatus( this );
		}

		private async void Image_OpenRequested( object sender, EventArgs args )
		{
			var selected = sender as StatusMediaViewModel;
			Debug.Assert( selected != null );

			var allUris = InlineMedias.ToList();
			var selectedUri = selected;

			await ViewServiceRepository.ViewImage( allUris, selectedUri );
		}

		private async Task LoadCard()
		{
			if( Entities?.UrlEntities == null )
			{
				return;
			}

			TwitterCard card = null;

			foreach( var url in Entities.UrlEntities.Select( u => u.ExpandedUrl ) )
			{
				card = await CardExtractor.ExtractCard( new Uri( url ) );
				if( card != null )
				{
					break;
				}
			}

			if( card != null )
			{
				Card = new CardViewModel( card );
				HasCard = Card.Card.IsValid;
			}
			else
			{
				Card = null;
				HasCard = false;
			}
		}

		private async Task LoadInlineMedias()
		{
			if( Config?.Visual?.InlineMedia != true )
			{
				return;
			}

			var videos = ( Model.ExtendedEntities?.MediaEntities.Where( e => e.Type == "animated_gif" || e.Type == "video" ) ??
			               Enumerable.Empty<MediaEntity>() ).ToArray();

			var entities = Model.Entities?.MediaEntities?.Concat( Model.ExtendedEntities?.MediaEntities ?? Enumerable.Empty<MediaEntity>() )
				               .Distinct( TwitterComparers.MediaEntityComparer ).Except( videos, TwitterComparers.MediaEntityComparer )
			               ?? Enumerable.Empty<MediaEntity>();

			entities = entities.Concat( videos );

			foreach( var vm in entities.Select( entity => new StatusMediaViewModel( entity ) ) )
			{
				vm.OpenRequested += Image_OpenRequested;
				_InlineMedias.Add( vm );
			}

			var urls = Model.Entities?.UrlEntities?.Concat( Model?.ExtendedEntities?.UrlEntities ?? Enumerable.Empty<UrlEntity>() )
				           .Distinct( TwitterComparers.UrlEntityComparer ).Select( e => e.ExpandedUrl )
			           ?? Enumerable.Empty<string>();

			foreach( var url in urls )
			{
				var extracted = await MediaExtractor.ExtractMedia( url );
				if( extracted != null )
				{
					var vm = new StatusMediaViewModel( extracted, new Uri( url ) );
					vm.OpenRequested += Image_OpenRequested;
					_InlineMedias.Add( vm );
				}
			}

			RaisePropertyChanged( nameof( InlineMedias ) );
		}

		private async Task LoadQuotedTweet()
		{
			var quoteId = ExtractQuotedTweetUrl();
			if( quoteId != 0 )
			{
				var quoted = await Context.Twitter.Statuses.GetTweet( quoteId, true );
				if( quoted != null )
				{
					QuotedTweet = new StatusViewModel( quoted, Context, Config, ViewServiceRepository );
				}
			}
		}

		public ICommand BlockUserCommand
			=>
			_BlockUserCommand ?? ( _BlockUserCommand = new RelayCommand( ExecuteBlockUserCommand, CanExecuteBlockUserCommand ) )
			;

		public CardViewModel Card
		{
			[DebuggerStepThrough] get { return _Card; }
			set
			{
				if( _Card == value )
				{
					return;
				}

				_Card = value;
				RaisePropertyChanged( nameof( Card ) );
			}
		}

		public ITwitterCardExtractor CardExtractor
		{
			get { return _CardExtractor ?? DefaultCardExtractor; }
			set { _CardExtractor = value; }
		}

		public IClipboard Clipboard
		{
			get { return _Clipboard ?? DefaultClipboard; }
			set { _Clipboard = value; }
		}

		public IContextEntry Context { get; }

		public ICommand CopyTweetCommand
			=> _CopyTweetCommand ?? ( _CopyTweetCommand = new RelayCommand( ExecuteCopyTweetCommand ) );

		public ICommand CopyTweetUrlCommand
			=> _CopyTweetUrlCommand ?? ( _CopyTweetUrlCommand = new RelayCommand( ExecuteCopyTweetUrlCommand ) );

		public override DateTime CreatedAt => Model.CreatedAt;

		public ICommand DeleteStatusCommand
			=>
			_DeleteStatusCommand
			?? ( _DeleteStatusCommand = new RelayCommand( ExecuteDeleteStatusCommand, CanExecuteDeleteStatusCommand ) );

		public IDispatcher Dispatcher { get; set; }

		public bool DisplayMedia => InlineMedias.Any();

		public override Entities Entities
		{
			get
			{
				var ent = Model?.Entities;
				var ext = Model?.ExtendedEntities;

				if( ent == null )
				{
					ent = new Entities();
				}
				EnsureEntitiesAreNotNull( ent );

				if( ext == null )
				{
					ext = new Entities();
				}
				EnsureEntitiesAreNotNull( ext );

				return new Entities
				{
					HashTagEntities = ent.HashTagEntities.Concat( ext.HashTagEntities ).ToList(),
					MediaEntities = ent.MediaEntities.Concat( ext.MediaEntities ).ToList(),
					SymbolEntities = ent.SymbolEntities.Concat( ext.SymbolEntities ).ToList(),
					UrlEntities = ent.UrlEntities.Concat( ext.UrlEntities ).ToList(),
					UserMentionEntities = ent.UserMentionEntities.Concat( ext.UserMentionEntities ).ToList()
				};
			}
		}

		public ICommand FavoriteStatusCommand
			=> _FavoriteStatusCommand ?? ( _FavoriteStatusCommand = new RelayCommand( ExecuteFavoriteStatusCommand ) );

		public bool HasCard
		{
			[DebuggerStepThrough] get { return _HasCard; }
			private set
			{
				if( _HasCard == value )
				{
					return;
				}

				_HasCard = value;
				RaisePropertyChanged( nameof( HasCard ) );
			}
		}

		public bool HasQuotedTweet => ExtractQuotedTweetUrl() != 0;

		public override ulong Id => Model.StatusID;

		public IEnumerable<StatusMediaViewModel> InlineMedias => _InlineMedias;

		public bool IsFavorited => Model.Favorited;

		public bool IsReply => Model.InReplyToStatusID != 0;

		public bool IsRetweeted => Model.Retweeted;

		public IMediaExtractorRepository MediaExtractor
		{
			get { return _MediaExtractor ?? DefaultMediaExtractor; }
			set { _MediaExtractor = value; }
		}

		public Status Model { get; }

		public override ulong OrderId => OriginalStatus.GetStatusId();

		public StatusViewModel QuotedTweet { get; set; }

		public ICommand QuoteStatusCommand
			=>
			_QuoteStatusCommand
			?? ( _QuoteStatusCommand = new RelayCommand( ExecuteQuoteStatusCommand ) );

		public ICommand ReplyCommand => _ReplyCommand ?? ( _ReplyCommand = new RelayCommand( ExecuteReplyCommand ) );

		public ICommand ReplyToAllCommand
			=>
			_ReplyToAllCommand
			?? ( _ReplyToAllCommand = new RelayCommand( ExecuteReplyToAllCommand, CanExecuteReplyToAllCommand ) );

		public ICommand ReportSpamCommand
			=>
			_ReportSpamCommand
			?? ( _ReportSpamCommand = new RelayCommand( ExecuteReportSpamCommand, CanExecuteReportSpamCommand ) );

		public ICollection<UserViewModel> RetweetedBy { get; }

		public ICommand RetweetStatusCommand
			=>
			_RetweetStatusCommand
			?? ( _RetweetStatusCommand = new RelayCommand( ExecuteRetweetStatusCommand ) );

		public UserViewModel SourceUser { get; }

		public override string Text => Model.Text;

		private static readonly ITwitterCardExtractor DefaultCardExtractor = TwitterCardExtractor.Default;

		private static readonly IClipboard DefaultClipboard = new ClipboardWrapper();

		private static readonly IMediaExtractorRepository DefaultMediaExtractor = MediaExtractorRepository.Default;

		private readonly List<StatusMediaViewModel> _InlineMedias = new List<StatusMediaViewModel>();

		private readonly IConfig Config;

		private readonly Status OriginalStatus;

		private readonly IViewServiceRepository ViewServiceRepository;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _BlockUserCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private CardViewModel _Card;

		private ITwitterCardExtractor _CardExtractor;

		private IClipboard _Clipboard;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _CopyTweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _CopyTweetUrlCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _DeleteStatusCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _FavoriteStatusCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _HasCard;

		private IMediaExtractorRepository _MediaExtractor;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _QuoteStatusCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _ReplyCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _ReplyToAllCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _ReportSpamCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _RetweetStatusCommand;
	}
}