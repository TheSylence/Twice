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
using Fody;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using LinqToTwitter;
using Ninject;
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
		public ComposeTweetViewModel( IScheduleInformationViewModel scheduler )
		{
			Accounts = new List<AccountEntry>();
			Title = Strings.ComposeTweet;

			ScheduleInformation = scheduler;
		}

		private void Acc_UseChanged( object sender, EventArgs e )
		{
			RaisePropertyChanged( nameof(ConfirmationRequired) );
		}

		private async Task AttachImage( string fileName )
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

			if( ScheduleInformation?.IsTweetScheduled == true )
			{
				var accountIds = Accounts.Where( a => a.Use ).Select( a => a.Context.UserId );
				var fileNames = AttachedMedias.Select( m => m.FileName );
				ScheduleInformation.ScheduleTweet( Text, InReplyTo?.Id, accountIds, fileNames );
			}
			else
			{
				var statuses = await SendTweet();
				statusIds.AddRange( statuses );
			}

			if( ScheduleInformation?.IsDeletionScheduled == true )
			{
				ScheduleInformation.ScheduleDeletion( statusIds, Text );
			}

			if( ScheduleInformation?.IsTweetScheduled == true )
			{
				await CloseOrReload();
			}

			IsSending = false;
		}

		private void InitializeText()
		{
			if( InReplyTo == null )
			{
				Text = InitialText;
				return;
			}

			List<string> mentions = new List<string> {InReplyTo.User.ScreenName};

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

		[UsedImplicitly]
		private void OnQuotedTweetChanged()
		{
			UpdateTextLength();
		}

		[UsedImplicitly]
		private void OnTextChanged()
		{
			UpdateTextLength();
		}

		private void OnTextLengthChanged()
		{
			LowCharsLeft = TextLength >= LowWarnThreshold;
			MediumCharsLeft = TextLength >= MediumWarnThreshold;
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

		private void UpdateTextLength()
		{
			var len = TwitterHelper.CountCharacters( Text, TwitterConfig );
			if( QuotedTweet != null )
			{
				// Keep the space in mind that separates the tweet text and the status URL
				len += TwitterConfig.UrlLengthHttps + 1;
			}
			TextLength = len;
		}

		public ICollection<AccountEntry> Accounts { get; private set; }
		public IList<MediaItem> AttachedMedias { get; } = new ObservableCollection<MediaItem>();

		public ICommand AttachImageCommand
			=> _AttachImageCommand ?? ( _AttachImageCommand = new RelayCommand<string>( ExecuteAttachImageCommand ) );

		public bool ConfirmationRequired
		{
			get { return Accounts.Where( a => a.Use ).Any( a => a.Context.RequiresConfirmation ); }
		}

		public bool ConfirmationSet { get; set; }

		public ICommand DeleteMediaCommand => _DeleteMediaCommand ?? ( _DeleteMediaCommand = new RelayCommand<ulong>(
			                                      ExecuteDeleteMediaCommand ) );

		public StatusViewModel InReplyTo { get; set; }

		public bool IsSending { get; set; }

		public ICollection<string> KnownHashtags { get; private set; }
		public ICollection<string> KnownUserNames { get; private set; }

		public bool LowCharsLeft { get; set; }

		public bool MediumCharsLeft { get; set; }

		public void PreSelectAccounts( IEnumerable<ulong> accounts )
		{
			PreSelectedAccounts = accounts.ToArray();
		}

		public StatusViewModel QuotedTweet { get; set; }

		public ICommand RemoveQuoteCommand
			=>
				_RemoveQuoteCommand
				?? ( _RemoveQuoteCommand = new RelayCommand( ExecuteRemoveQuoteCommand, CanExecuteRemoveQuoteCommand ) );

		public ICommand RemoveReplyCommand => _RemoveReplyCommand ?? ( _RemoveReplyCommand = new RelayCommand(
			                                      ExecuteRemoveReplyCommand ) );

		public IScheduleInformationViewModel ScheduleInformation { get; }

		public ICommand SendTweetCommand
			=>
				_SendTweetCommand ?? ( _SendTweetCommand = new RelayCommand( ExecuteSendTweetCommand, CanExecuteSendTweetCommand ) );

		public void SetInitialText( string text )
		{
			InitialText = text;
		}

		public void SetReply( StatusViewModel status, bool toAll )
		{
			InReplyTo = status;
			ReplyToAll = toAll;
		}

		public bool StayOpen { get; set; }
		public string Text { get; set; }

		public int TextLength { get; set; }

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

			RaisePropertyChanged( nameof(Accounts) );

			InitializeText();
			ConfirmationSet = false;

			Medias.Clear();
			AttachedMedias.Clear();

			KnownUserNames = ( await Cache.GetKnownUsers().ConfigureAwait( false ) ).Select( u => u.UserName ).ToList();
			RaisePropertyChanged( nameof(KnownUserNames) );
			KnownHashtags = ( await Cache.GetKnownHashtags().ConfigureAwait( false ) ).ToList();
			RaisePropertyChanged( nameof(KnownHashtags) );

			ScheduleInformation?.ResetSchedule();
		}

		private const int LowWarnThreshold = 135;
		private const int MediumWarnThreshold = 125;

		[Inject]
		public INotifier Notifier { get; set; }

		private readonly List<Media> Medias = new List<Media>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand<string> _AttachImageCommand;

		private RelayCommand<ulong> _DeleteMediaCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _RemoveQuoteCommand;

		private RelayCommand _RemoveReplyCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _SendTweetCommand;

		private string InitialText = string.Empty;
		private ulong[] PreSelectedAccounts = new ulong[0];
		private bool ReplyToAll;
	}
}