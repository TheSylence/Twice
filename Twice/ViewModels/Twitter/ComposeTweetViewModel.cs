using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.Services;
using Twice.Services.Views;

namespace Twice.ViewModels.Twitter
{
	internal interface IComposeTweetViewModel : IResetable
	{
		ICollection<AccountEntry> Accounts { get; }
		ICommand AttachImageCommand { get; }
		bool IsSending { get; }
		ICommand SendTweetCommand { get; }
		string Text { get; set; }
	}

	internal class ComposeTweetViewModel : ViewModelBaseEx, IComposeTweetViewModel
	{
		public ComposeTweetViewModel()
		{
		}

		public void Reset()
		{
			Accounts = new List<AccountEntry>( ContextList.Contexts.Select( c => new AccountEntry( c ) ) );
			Accounts.First().Use = true;

			Text = string.Empty;

			Medias.Clear();
		}

		private static string GetMimeType( string fileName )
		{
			var ext = Path.GetExtension( fileName );

			var lookup = new Dictionary<string, string>
			{
				{".png", "image/png"}, {".gif", "image/gif"}, {".bmp", "image/bmp"}
			};

			if( ext != null && lookup.ContainsKey( ext ) )
			{
				return lookup[ext];
			}

			return "application/octet-stream";
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

			if( TwitterHelper.CountCharacters( Text ) > Constants.Twitter.MaxTweetLength )
			{
				return false;
			}

			return true;
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
			} ).ContinueWith( t =>
			{
				IsSending = false;
				Reset();
			} );
		}

		public ICollection<AccountEntry> Accounts { get; private set; }
		public ICommand AttachImageCommand => _AttachImageCommand ?? ( _AttachImageCommand = new RelayCommand( ExecuteAttachImageCommand, CanExecuteAttachImageCommand ) );

		public bool IsSending
		{
			[DebuggerStepThrough]
			get
			{
				return _IsSending;
			}
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

		public ICommand SendTweetCommand => _SendTweetCommand ?? ( _SendTweetCommand = new RelayCommand( ExecuteSendTweetCommand, CanExecuteSendTweetCommand ) );

		public string Text
		{
			[DebuggerStepThrough]
			get
			{
				return _Text;
			}
			set
			{
				if( _Text == value )
				{
					return;
				}

				_Text = value;
				RaisePropertyChanged();
			}
		}

		private readonly List<Media> Medias = new List<Media>();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _AttachImageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsSending;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _SendTweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Text;

		private float LowWarnThreshold = 0.05f;
		private float MediumWarnThreshold = 0.1f;
	}
}