using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Twitter
{
	internal interface IComposeTweetViewModel : IResetable
	{
		ICollection<AccountEntry> Accounts { get; }
		ICommand SendTweetCommand { get; }
		string Text { get; set; }
		bool IsSending { get; }
	}

	internal class AccountEntry : ObservableObject
	{
		public AccountEntry( IContextEntry context )
		{
			Context = context;
			Image = new BitmapImage( context.ProfileImageUrl );
		}

		public IContextEntry Context { get; }
		public ImageSource Image { get; }
		public string ScreenName => Context.AccountName;

		public bool Use
		{
			[DebuggerStepThrough]
			get
			{
				return _Use;
			}
			set
			{
				if( _Use == value )
				{
					return;
				}

				_Use = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _Use;
	}

	internal class ComposeTweetViewModel : ViewModelBaseEx, IComposeTweetViewModel
	{
		public void Reset()
		{
			Text = string.Empty;

			Accounts = new List<AccountEntry>( ContextList.Contexts.Select( c => new AccountEntry( c ) ) );
			RaisePropertyChanged( nameof( Accounts ) );
		}

		private bool CanExecuteSendTweetCommand()
		{
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

		private async Task SendTweet()
		{
			IsSending = true;

			await Task.Run( async () =>
			{
				foreach( var acc in Accounts.Where( a => a.Use ) )
				{
					await acc.Context.Twitter.TweetAsync( Text );
				}
			} ).ContinueWith( t =>
			{
				IsSending = false;
			});
		}


		[System.Diagnostics.DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private bool _IsSending;

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

		private async void ExecuteSendTweetCommand()
		{
			await SendTweet();
		}

		public ICollection<AccountEntry> Accounts { get; private set; }
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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _SendTweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string _Text;

		private float LowWarnThreshold = 0.05f;
		private float MediumWarnThreshold = 0.1f;
	}
}