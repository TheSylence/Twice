using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Messages;
using Twice.Models.Cache;
using Twice.Models.Twitter;
using Twice.Services.Views;
using Twice.Views;

namespace Twice.ViewModels.Twitter
{
	// ReSharper disable once ClassNeverInstantiated.Global
	internal class ComposeTweetViewModel : ViewModelBaseEx, IComposeTweetViewModel
	{
		public ComposeTweetViewModel( IDataCache cache )
		{
			Cache = cache;
			Accounts = new List<AccountEntry>();
		}

		public async Task Reset()
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

			KnownUserNames = ( await Cache.GetKnownUsers() ).Select( u => u.Name ).ToList();
			RaisePropertyChanged( nameof( KnownUserNames ) );
			KnownHashtags = ( await Cache.GetKnownHashtags() ).ToList();
			RaisePropertyChanged( nameof( KnownHashtags ) );
		}

		private static string GetMimeType( string fileName )
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

		private void Acc_UseChanged( object sender, System.EventArgs e )
		{
			RaisePropertyChanged( nameof( ConfirmationRequired ) );
		}

		private bool CanExecuteAttachImageCommand()
		{
			return true;
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

			return TwitterHelper.CountCharacters( Text ) <= Constants.Twitter.MaxTweetLength;
		}

		private async void ExecuteAttachImageCommand()
		{
			var fsa = new FileServiceArgs( "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif" );
			var selectedFile = await ViewServiceRepository.OpenFile( fsa );
			if( selectedFile == null )
			{
				return;
			}

			var usedAccounts = Accounts.Where( a => a.Use ).ToArray();

			var acc = usedAccounts.First();
			var additionalOwners = usedAccounts.Skip( 1 ).Select( a => a.Context.UserId );

			IsSending = true;

			string mediaType = GetMimeType( selectedFile );
			byte[] mediaData = File.ReadAllBytes( selectedFile );

			var media = await acc.Context.Twitter.UploadMediaAsync( mediaData, mediaType, additionalOwners ).ContinueWith( t =>
			{
				IsSending = false;
				return t.Result;
			} );

			Medias.Add( media );
			AttachedMedias.Add( new MediaItem( media, mediaData ) );
		}

		private async void ExecuteSendTweetCommand()
		{
			await SendTweet();
		}

		private async Task SendTweet()
		{
			IsSending = true;

			await Task.Run( async () =>
			{
				foreach( var acc in Accounts.Where( a => a.Use ) )
				{
					await acc.Context.Twitter.TweetAsync( Text, Medias.Select( m => m.MediaID ) );
				}
			} ).ContinueWith( async t =>
			{
				IsSending = false;
				MessengerInstance.Send( new FlyoutMessage( FlyoutNames.TweetComposer, FlyoutAction.Close ) );
				await Reset();
			} );
		}

		public ICollection<AccountEntry> Accounts { get; private set; }

		public ICollection<MediaItem> AttachedMedias { get; } = new ObservableCollection<MediaItem>();

		public ICommand AttachImageCommand
			=> _AttachImageCommand ?? ( _AttachImageCommand = new RelayCommand( ExecuteAttachImageCommand, CanExecuteAttachImageCommand ) );

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

		public ICommand SendTweetCommand => _SendTweetCommand ?? ( _SendTweetCommand = new RelayCommand( ExecuteSendTweetCommand, CanExecuteSendTweetCommand ) );

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
				TextLength = TwitterHelper.CountCharacters( Text );
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

		private readonly IDataCache Cache;

		private readonly int LowWarnThreshold = 135;

		private readonly List<Media> Medias = new List<Media>();

		private readonly int MediumWarnThreshold = 125;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _AttachImageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _ConfirmationSet;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsSending;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _LowCharsLeft;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _MediumCharsLeft;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _SendTweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Text;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private int _TextLength;
	}
}