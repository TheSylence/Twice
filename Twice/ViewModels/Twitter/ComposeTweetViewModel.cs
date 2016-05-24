using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Fody;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using LinqToTwitter;
using Ninject;
using Twice.Messages;
using Twice.Models.Cache;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.Services.Views;
using Twice.Views;

namespace Twice.ViewModels.Twitter
{
	// ReSharper disable once ClassNeverInstantiated.Global
	[ConfigureAwait( false )]
	internal class ComposeTweetViewModel : ViewModelBaseEx, IComposeTweetViewModel
	{
		public ComposeTweetViewModel()
		{
			Accounts = new List<AccountEntry>();
		}

		public async Task OnLoad( object data )
		{
			foreach( var acc in Accounts )
			{
				acc.UseChanged -= Acc_UseChanged;
			}

			Accounts = ContextList.Contexts.Select( c => new AccountEntry( c ) ).ToList();
			foreach( var acc in Accounts )
			{
				acc.UseChanged += Acc_UseChanged;
			}

			var defAccount = Accounts.FirstOrDefault( a => a.IsDefault ) ?? Accounts.First();
			defAccount.Use = true;
			RaisePropertyChanged( nameof( Accounts ) );

			Text = string.Empty;
			ConfirmationSet = false;

			Medias.Clear();
			AttachedMedias.Clear();

			KnownUserNames = ( await Cache.GetKnownUsers().ConfigureAwait( false ) ).Select( u => u.UserName ).ToList();
			RaisePropertyChanged( nameof( KnownUserNames ) );
			KnownHashtags = ( await Cache.GetKnownHashtags().ConfigureAwait( false ) ).ToList();
			RaisePropertyChanged( nameof( KnownHashtags ) );
		}

		internal static string GetMimeType( string fileName )
		{
			var ext = Path.GetExtension( fileName );

			var lookup = new Dictionary<string, string>
			{
				{".png", "image/png"},
				{".gif", "image/gif"},
				{".bmp", "image/bmp"}
			};

			if( ext != null && lookup.ContainsKey( ext ) )
			{
				return lookup[ext];
			}

			return "application/octet-stream";
		}

		private void Acc_UseChanged( object sender, EventArgs e )
		{
			RaisePropertyChanged( nameof( ConfirmationRequired ) );
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

		private async void ExecuteAttachImageCommand()
		{
			var fsa = new FileServiceArgs( "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif" );
			var selectedFile = await ViewServiceRepository.OpenFile( fsa ).ConfigureAwait( false );
			if( selectedFile == null )
			{
				return;
			}

			IsSending = true;

			byte[] mediaData = File.ReadAllBytes( selectedFile );
			if( mediaData.Length > TwitterConfig.MaxImageSize )
			{
				Notifier.DisplayMessage( Strings.ImageSizeTooBig, NotificationType.Error );
				IsSending = false;
				return;
			}

			var usedAccounts = Accounts.Where( a => a.Use ).ToArray();

			var acc = usedAccounts.First();
			var additionalOwners = usedAccounts.Skip( 1 ).Select( a => a.Context.UserId );

			string mediaType = GetMimeType( selectedFile );
			var media = await acc.Context.Twitter.UploadMediaAsync( mediaData, mediaType, additionalOwners ).ContinueWith( t =>
			{
				IsSending = false;
				return t.Result;
			} );

			Medias.Add( media );
			AttachedMedias.Add( new MediaItem( media.MediaID, mediaData ) );
		}

		private void ExecuteDeleteMediaCommand( ulong id )
		{
			// TODO: Confirm removal
			Medias.RemoveAll( m => m.MediaID == id );
			for( int i = 0; i < AttachedMedias.Count; ++i )
			{
				if( AttachedMedias[i].MediaId == id )
				{
					AttachedMedias.RemoveAt( i );
					break;
				}
			}
		}

		private void ExecuteRemoveQuoteCommand()
		{
			QuotedTweet = null;
		}

		private async void ExecuteSendTweetCommand()
		{
			await SendTweet();
		}

		private async Task SendTweet()
		{
			IsSending = true;

			var textToTweet = Text;
			if( QuotedTweet != null )
			{
				textToTweet += " " + QuotedTweet.Model.GetUrl().AbsoluteUri;
			}

			await Task.Run( async () =>
			{
				foreach( var acc in Accounts.Where( a => a.Use ) )
				{
					await acc.Context.Twitter.TweetAsync( textToTweet, Medias.Select( m => m.MediaID ) ).ConfigureAwait( false );
				}
			} ).ContinueWith( async t =>
			{
				IsSending = false;
				await DispatcherHelper.RunAsync( async () =>
				{
					if( !StayOpen )
					{
						MessengerInstance.Send( new FlyoutMessage( FlyoutNames.TweetComposer, FlyoutAction.Close ) );
					}

					await OnLoad( null );
				} );
			} );
		}

		public ICollection<AccountEntry> Accounts { get; private set; }

		public IList<MediaItem> AttachedMedias { get; } = new ObservableCollection<MediaItem>();

		public ICommand AttachImageCommand
			=> _AttachImageCommand ?? ( _AttachImageCommand = new RelayCommand( ExecuteAttachImageCommand ) );

		[Inject]
		public ICache Cache { get; set; }

		public bool ConfirmationRequired
		{
			get { return Accounts.Where( a => a.Use ).Any( a => a.Context.RequiresConfirmation ); }
		}

		public bool ConfirmationSet
		{
			[DebuggerStepThrough] get { return _ConfirmationSet; }
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

		public bool IsSending
		{
			[DebuggerStepThrough] get { return _IsSending; }
			set
			{
				if( _IsSending == value )
				{
					return;
				}

				_IsSending = value;
				RaisePropertyChanged();
			}
		}

		public ICollection<string> KnownHashtags { get; private set; }

		public ICollection<string> KnownUserNames { get; private set; }

		public bool LowCharsLeft
		{
			[DebuggerStepThrough] get { return _LowCharsLeft; }
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
			[DebuggerStepThrough] get { return _MediumCharsLeft; }
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
			[DebuggerStepThrough] get { return _QuotedTweet; }
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

		public ICommand SendTweetCommand
			=>
				_SendTweetCommand ?? ( _SendTweetCommand = new RelayCommand( ExecuteSendTweetCommand, CanExecuteSendTweetCommand ) )
			;

		public bool StayOpen
		{
			[DebuggerStepThrough] get { return _StayOpen; }
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
			[DebuggerStepThrough] get { return _Text; }
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
			[DebuggerStepThrough] get { return _TextLength; }
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

		private readonly int LowWarnThreshold = 135;

		private readonly List<Media> Medias = new List<Media>();

		private readonly int MediumWarnThreshold = 125;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _AttachImageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _ConfirmationSet;

		private RelayCommand<ulong> _DeleteMediaCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsSending;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _LowCharsLeft;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _MediumCharsLeft;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private StatusViewModel _QuotedTweet;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _RemoveQuoteCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _SendTweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _StayOpen;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Text;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private int _TextLength;
	}
}