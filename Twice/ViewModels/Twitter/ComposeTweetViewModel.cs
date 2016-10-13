using Fody;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;
using LinqToTwitter;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Anotar.NLog;
using Twice.Models.Scheduling;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.Views.Services;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace Twice.ViewModels.Twitter
{
	// ReSharper disable once ClassNeverInstantiated.Global
	[ConfigureAwait( false )]
	internal class ComposeTweetViewModel : DialogViewModel, IComposeTweetViewModel
	{
		public ComposeTweetViewModel()
		{
			Accounts = new List<AccountEntry>();

			Validate( () => ScheduleDate )
				.If( () => IsTweetScheduled )
				.Check( dt => dt.Date >= DateTime.Now.Date )
				.Message( Strings.DateMustBeInTheFuture );

			Validate( () => ScheduleTime )
				.If( () => IsTweetScheduled )
				.Check( dt => dt.TimeOfDay >= DateTime.Now.TimeOfDay || ScheduleDate.Date > DateTime.Now.Date )
				.Message( Strings.DateMustBeInTheFuture );

			Validate( () => DeletionDate )
				.If( () => IsDeletionScheduled )
				.Check( dt => dt.Date >= DateTime.Now.Date )
				.Message( Strings.DateMustBeInTheFuture );

			Validate( () => DeletionTime )
				.If( () => IsDeletionScheduled )
				.Check( dt => dt.TimeOfDay >= DateTime.Now.TimeOfDay || DeletionDate.Date > DateTime.Now.Date )
				.Message( Strings.DateMustBeInTheFuture );

			Validate( () => DeletionDate )
				.If( () => IsDeletionScheduled && IsTweetScheduled )
				.Check( dt => dt.Date >= ScheduleDate.Date )
				.Message( Strings.DateMustBeInTheFuture );

			Validate( () => DeletionTime )
				.If( () => IsDeletionScheduled && IsTweetScheduled )
				.Check( dt => dt.TimeOfDay >= ScheduleDate.TimeOfDay || DeletionDate.Date > ScheduleDate.Date )
				.Message( Strings.DateMustBeInTheFuture );
		}

		public event EventHandler ScrollToEnd;

		public void DragOver( IDropInfo dropInfo )
		{
			var data = dropInfo.Data as DataObject;
			if( data == null )
			{
				return;
			}

			if( data.ContainsFileDropList() )
			{
				var files = data.GetFileDropList();
				bool canAttach = false;

				foreach( var file in files )
				{
					if( TwitterHelper.IsSupportedImage( file ) )
					{
						canAttach = true;
					}
				}

				if( canAttach )
				{
					dropInfo.Effects = DragDropEffects.Copy;
				}
			}

			DragDrop.DefaultDropHandler.DragOver( dropInfo );
		}

		public async void Drop( IDropInfo dropInfo )
		{
			var data = dropInfo.Data as DataObject;
			if( data == null )
			{
				return;
			}

			if( data.ContainsFileDropList() )
			{
				var files = data.GetFileDropList();

				foreach( var file in files )
				{
					if( TwitterHelper.IsSupportedImage( file ) )
					{
						await AttachImage( file );
					}
				}
			}

			DragDrop.DefaultDropHandler.Drop( dropInfo );
		}

		public async Task OnLoad( object data )
		{
			bool loadImage = data as bool? ?? true;

			foreach( var acc in Accounts )
			{
				acc.UseChanged -= Acc_UseChanged;
			}

			Accounts = ContextList?.Contexts?.Select( c => new AccountEntry( c, loadImage ) ).ToList();
			if( Accounts == null )
			{
				LogTo.Warn( "No accounts found" );
				return;
			}

			foreach( var acc in Accounts )
			{
				acc.UseChanged += Acc_UseChanged;
			}

			var defAccount = Accounts.FirstOrDefault( a => a.IsDefault ) ?? Accounts.First();
			defAccount.Use = true;

			if( PreSelectedAccounts.Any() )
			{
				foreach( var acc in Accounts )
				{
					acc.Use = PreSelectedAccounts.Contains( acc.Context.UserId );
				}
			}

			RaisePropertyChanged( nameof( Accounts ) );

			InitializeText();
			ConfirmationSet = false;

			Medias.Clear();
			AttachedMedias.Clear();

			KnownUserNames = ( await Cache.GetKnownUsers().ConfigureAwait( false ) ).Select( u => u.UserName ).ToList();
			RaisePropertyChanged( nameof( KnownUserNames ) );
			KnownHashtags = ( await Cache.GetKnownHashtags().ConfigureAwait( false ) ).ToList();
			RaisePropertyChanged( nameof( KnownHashtags ) );

			ScheduleDate = DateTime.Now;
			ScheduleTime = DateTime.Now;
			DeletionDate = DateTime.Now;
			DeletionTime = DateTime.Now;
			IsTweetScheduled = false;
			IsDeletionScheduled = false;
		}

		public void PreSelectAccounts( IEnumerable<ulong> accounts )
		{
			PreSelectedAccounts = accounts.ToArray();
		}

		public void SetReply( StatusViewModel status, bool toAll )
		{
			InReplyTo = status;
			ReplyToAll = toAll;
		}

		private void Acc_UseChanged( object sender, EventArgs e )
		{
			RaisePropertyChanged( nameof( ConfirmationRequired ) );
		}

		public async Task AttachImage( string fileName )
		{
			IsSending = true;

			if( Path.GetExtension( fileName ) == ".gif" )
			{
				if( GifValidator.Validate( fileName ) != GifValidator.ValidationResult.Ok )
				{
					Notifier.DisplayMessage( Strings.ImageSizeTooBig, NotificationType.Error );
					IsSending = false;
					return;
				}
			}

			byte[] mediaData = File.ReadAllBytes( fileName );
			if( mediaData.Length > TwitterConfig.MaxImageSize )
			{
				Notifier.DisplayMessage( Strings.ImageSizeTooBig, NotificationType.Error );
				IsSending = false;
				return;
			}

			var usedAccounts = Accounts.Where( a => a.Use ).ToArray();
			var acc = usedAccounts.First();
			var additionalOwners = usedAccounts.Skip( 1 ).Select( a => a.Context.UserId );

			string mediaType = TwitterHelper.GetMimeType( fileName );
			var media = await acc.Context.Twitter.UploadMediaAsync( mediaData, mediaType, additionalOwners ).ContinueWith( t =>
			{
				IsSending = false;
				return t.Result;
			} );

			await Dispatcher.RunAsync( () =>
			{
				Medias.Add( media );
				AttachedMedias.Add( new MediaItem( media.MediaID, mediaData, fileName ) );
			} );
		}

		private bool CanExecuteRemoveQuoteCommand()
		{
			return QuotedTweet != null;
		}

		private bool CanExecuteSendTweetCommand()
		{
			if( IsSending )
			{
				return false;
			}

			if( string.IsNullOrWhiteSpace( Text ) )
			{
				return false;
			}

			if( !Accounts.Any( a => a.Use ) )
			{
				return false;
			}

			if( ConfirmationRequired && !ConfirmationSet )
			{
				return false;
			}

			return TwitterHelper.CountCharacters( Text, TwitterConfig ) <= Constants.Twitter.MaxTweetLength;
		}

		private async Task CloseOrReload()
		{
			await Dispatcher.RunAsync( async () =>
			{
				if( !StayOpen )
				{
					Close( true );
				}

				await OnLoad( null );
			} );
		}

		private async void ExecuteAttachImageCommand( string fileName )
		{
			string selectedFile = fileName;

			if( selectedFile == null )
			{
				var fsa = new FileServiceArgs( "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif" );
				selectedFile = await ViewServiceRepository.OpenFile( fsa ).ConfigureAwait( false );
			}

			if( selectedFile == null )
			{
				return;
			}

			await AttachImage( selectedFile );
		}

		private async void ExecuteDeleteMediaCommand( ulong id )
		{
			var csa = new ConfirmServiceArgs( Strings.ConfirmMediaRemoval );
			if( !await ViewServiceRepository.Confirm( csa ) )
			{
				return;
			}

			Medias.RemoveAll( m => m.MediaID == id );
			for( int i = 0; i < AttachedMedias.Count; ++i )
			{
				if( AttachedMedias[i].MediaId == id )
				{
					await Dispatcher.RunAsync( () => AttachedMedias.RemoveAt( i ) );
					break;
				}
			}
		}

		private void ExecuteRemoveQuoteCommand()
		{
			QuotedTweet = null;
		}

		private void ExecuteRemoveReplyCommand()
		{
			InReplyTo = null;
			InitializeText();
		}

		private async void ExecuteSendTweetCommand()
		{
			IsSending = true;
			List<Tuple<ulong, ulong>> statusIds = new List<Tuple<ulong, ulong>>();

			if( IsTweetScheduled )
			{
				ScheduleTweet();
			}
			else
			{
				var statuses = await SendTweet();
				statusIds.AddRange( statuses );
			}

			if( IsDeletionScheduled )
			{
				ScheduleDeletion( statusIds );
			}

			if( IsTweetScheduled )
			{
				await CloseOrReload();
			}

			IsSending = false;
		}

		private void InitializeText()
		{
			if( InReplyTo == null )
			{
				Text = string.Empty;
				return;
			}

			List<string> mentions = new List<string> { InReplyTo.User.ScreenName };

			if( ReplyToAll )
			{
				foreach( var m in InReplyTo.Model.Entities.UserMentionEntities )
				{
					mentions.Add( Constants.Twitter.Mention + m.ScreenName );
				}

				mentions.Add( InReplyTo.SourceUser?.ScreenName );
			}

			var toAdd = mentions.Distinct().Where( m => !string.IsNullOrEmpty( m ) );

			var currentContextName = Constants.Twitter.Mention + InReplyTo.Context.AccountName;
			toAdd = toAdd.Where( m => m != currentContextName );

			Text = string.Join( " ", toAdd ) + " ";
			ScrollToEnd?.Invoke( this, EventArgs.Empty );
		}

		private void ScheduleDeletion( List<Tuple<ulong, ulong>> tweetIds )
		{
			var job = new SchedulerJob
			{
				JobType = SchedulerJobType.DeleteStatus,
				IdsToDelete = tweetIds.Select( t => t.Item1 ).ToList(),
				AccountIds = tweetIds.Select( t => t.Item2 ).ToList(),
				TargetTime = DeletionDate + DeletionTime.TimeOfDay,
				Text = Text
			};

			Scheduler.AddJob( job );
		}

		private void ScheduleTweet()
		{
			var job = new SchedulerJob
			{
				JobType = SchedulerJobType.CreateStatus,
				Text = Text,
				AccountIds = Accounts.Where( a => a.Use ).Select( a => a.Context.UserId ).ToList(),
				TargetTime = ScheduleDate + ScheduleTime.TimeOfDay,
				InReplyToStatus = InReplyTo?.Id ?? 0,
				FilesToAttach = AttachedMedias.Select( m => m.FileName ).ToList()
			};

			Scheduler.AddJob( job );
		}

		private async Task<List<Tuple<ulong, ulong>>> SendTweet()
		{
			var textToTweet = Text;
			if( QuotedTweet != null )
			{
				textToTweet += " " + QuotedTweet.Model.GetUrl().AbsoluteUri;
			}

			var result = new List<Tuple<ulong, ulong>>();

			await Task.Run( async () =>
			{
				ulong inReplyToId = InReplyTo?.Id ?? 0;

				foreach( var acc in Accounts.Where( a => a.Use ) )
				{
					var status = await
						acc.Context.Twitter.Statuses.TweetAsync( textToTweet, Medias.Select( m => m.MediaID ), inReplyToId )
							.ConfigureAwait( false );

					result.Add( new Tuple<ulong, ulong>( status.ID, acc.Context.UserId ) );
				}
			} ).ContinueWith( async t =>
			{
				if( t.IsFaulted )
				{
					Notifier.DisplayMessage( t.Exception?.GetReason(), NotificationType.Error );
					return;
				}

				await CloseOrReload();
			} ).ContinueWith( t => { IsSending = false; } );

			return result;
		}

		public ICollection<AccountEntry> Accounts { get; private set; }

		public IList<MediaItem> AttachedMedias { get; } = new ObservableCollection<MediaItem>();

		public ICommand AttachImageCommand
			=> _AttachImageCommand ?? ( _AttachImageCommand = new RelayCommand<string>( ExecuteAttachImageCommand ) );

		public bool ConfirmationRequired
		{
			get { return Accounts.Where( a => a.Use ).Any( a => a.Context.RequiresConfirmation ); }
		}

		public bool ConfirmationSet
		{
			[DebuggerStepThrough]
			get { return _ConfirmationSet; }
			set
			{
				if( _ConfirmationSet == value )
				{
					return;
				}

				_ConfirmationSet = value;
				RaisePropertyChanged();
			}
		}

		public ICommand DeleteMediaCommand => _DeleteMediaCommand ?? ( _DeleteMediaCommand = new RelayCommand<ulong>(
												  ExecuteDeleteMediaCommand ) );

		public DateTime DeletionDate
		{
			[DebuggerStepThrough]
			get { return _DeletionDate; }
			set
			{
				if( _DeletionDate == value )
				{
					return;
				}

				_DeletionDate = value;
				RaisePropertyChanged();
			}
		}

		public DateTime DeletionTime
		{
			[DebuggerStepThrough]
			get { return _DeletionTime; }
			set
			{
				if( _DeletionTime == value )
				{
					return;
				}

				_DeletionTime = value;
				RaisePropertyChanged();
			}
		}

		public StatusViewModel InReplyTo
		{
			[DebuggerStepThrough]
			get { return _InReplyTo; }
			set
			{
				if( _InReplyTo == value )
				{
					return;
				}

				_InReplyTo = value;
				RaisePropertyChanged();
			}
		}

		public bool IsDeletionScheduled
		{
			[DebuggerStepThrough]
			get { return _IsDeletionScheduled; }
			set
			{
				if( _IsDeletionScheduled == value )
				{
					return;
				}

				_IsDeletionScheduled = value;
				RaisePropertyChanged();

				if( value )
				{
					IsTweetScheduled = false;
				}
			}
		}

		public bool IsSending
		{
			[DebuggerStepThrough]
			get { return _IsSending; }
			private set
			{
				if( _IsSending == value )
				{
					return;
				}

				_IsSending = value;
				RaisePropertyChanged();
			}
		}

		public bool IsTweetScheduled
		{
			[DebuggerStepThrough]
			get { return _IsTweetScheduled; }
			set
			{
				if( _IsTweetScheduled == value )
				{
					return;
				}

				_IsTweetScheduled = value;
				RaisePropertyChanged();

				if( value )
				{
					IsDeletionScheduled = false;
				}
			}
		}

		public ICollection<string> KnownHashtags { get; private set; }

		public ICollection<string> KnownUserNames { get; private set; }

		public bool LowCharsLeft
		{
			[DebuggerStepThrough]
			get { return _LowCharsLeft; }
			set
			{
				if( _LowCharsLeft == value )
				{
					return;
				}

				_LowCharsLeft = value;
				RaisePropertyChanged();
			}
		}

		public bool MediumCharsLeft
		{
			[DebuggerStepThrough]
			get { return _MediumCharsLeft; }
			set
			{
				if( _MediumCharsLeft == value )
				{
					return;
				}

				_MediumCharsLeft = value;
				RaisePropertyChanged();
			}
		}

		[Inject]
		public INotifier Notifier { get; set; }

		public StatusViewModel QuotedTweet
		{
			[DebuggerStepThrough]
			get { return _QuotedTweet; }
			set
			{
				if( _QuotedTweet == value )
				{
					return;
				}

				_QuotedTweet = value;
				RaisePropertyChanged();
			}
		}

		public ICommand RemoveQuoteCommand
			=>
			_RemoveQuoteCommand
			?? ( _RemoveQuoteCommand = new RelayCommand( ExecuteRemoveQuoteCommand, CanExecuteRemoveQuoteCommand ) );

		public ICommand RemoveReplyCommand => _RemoveReplyCommand ?? ( _RemoveReplyCommand = new RelayCommand(
												  ExecuteRemoveReplyCommand ) );

		public DateTime ScheduleDate
		{
			[DebuggerStepThrough]
			get { return _ScheduleDate; }
			set
			{
				if( _ScheduleDate == value )
				{
					return;
				}

				_ScheduleDate = value;
				RaisePropertyChanged();
			}
		}

		[Inject]
		public IScheduler Scheduler { get; set; }

		public DateTime ScheduleTime
		{
			[DebuggerStepThrough]
			get { return _ScheduleTime; }
			set
			{
				if( _ScheduleTime == value )
				{
					return;
				}

				_ScheduleTime = value;
				RaisePropertyChanged();
			}
		}

		public ICommand SendTweetCommand
			=>
			_SendTweetCommand ?? ( _SendTweetCommand = new RelayCommand( ExecuteSendTweetCommand, CanExecuteSendTweetCommand ) )
			;

		public bool StayOpen
		{
			[DebuggerStepThrough]
			get { return _StayOpen; }
			set
			{
				if( _StayOpen == value )
				{
					return;
				}

				_StayOpen = value;
				RaisePropertyChanged();
			}
		}

		public string Text
		{
			[DebuggerStepThrough]
			get { return _Text; }
			set
			{
				if( _Text == value )
				{
					return;
				}

				_Text = value;
				RaisePropertyChanged();

				var len = TwitterHelper.CountCharacters( Text, TwitterConfig );
				if( QuotedTweet != null )
				{
					// Keep the space in mind that separates the tweet text and the status URL
					len += TwitterConfig.UrlLengthHttps + 1;
				}
				TextLength = len;
			}
		}

		public int TextLength
		{
			[DebuggerStepThrough]
			get { return _TextLength; }
			set
			{
				if( _TextLength == value )
				{
					return;
				}

				_TextLength = value;
				RaisePropertyChanged();

				LowCharsLeft = value >= LowWarnThreshold;
				MediumCharsLeft = value >= MediumWarnThreshold;
			}
		}

		private const int LowWarnThreshold = 135;

		private const int MediumWarnThreshold = 125;

		private readonly List<Media> Medias = new List<Media>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand<string> _AttachImageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _ConfirmationSet;

		private RelayCommand<ulong> _DeleteMediaCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private DateTime _DeletionDate;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private DateTime _DeletionTime;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private StatusViewModel _InReplyTo;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsDeletionScheduled;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsSending;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsTweetScheduled;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _LowCharsLeft;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _MediumCharsLeft;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private StatusViewModel _QuotedTweet;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _RemoveQuoteCommand;

		private RelayCommand _RemoveReplyCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private DateTime _ScheduleDate;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private DateTime _ScheduleTime;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _SendTweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _StayOpen;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Text;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private int _TextLength;

		private ulong[] PreSelectedAccounts = new ulong[0];
		private bool ReplyToAll;
	}
}