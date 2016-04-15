using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Anotar.NLog;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Resources;

namespace Twice.ViewModels.Accounts
{
	internal class AccountsDialogViewModel : DialogViewModel, IAccountsDialogViewModel
	{
		public AccountsDialogViewModel( IColumnDefinitionList columnList, ITwitterContextList contextList )
		{
			ContextList = contextList;
			ColumnList = columnList;

			AddedAccounts = ContextList.Contexts.Select( c => new AccountEntry( c ) ).ToList();
			foreach( var acc in AddedAccounts )
			{
				acc.ConfirmationChanged += Acc_ConfirmationChanged;
			}
		}

		private void Acc_ConfirmationChanged( object sender, EventArgs e )
		{
			var acc = sender as AccountEntry;
			Debug.Assert( acc != null, "acc != null" );

			acc.Data.ExecuteDecryptedAction( data => { ContextList.UpdateAccount( data ); } );
		}

		private void DisplayPinPage( string url )
		{
			// TODO: An in-app browser would be cleaner I guess
			Process.Start( url );
		}

		private async void ExecuteAddAccountCommand()
		{
			PinEntryCancelled = false;

			var auth = new PinAuthorizer
			{
				CredentialStore = new InMemoryCredentialStore
				{
					ConsumerKey = Constants.Auth.ConsumerKey,
					ConsumerSecret = Constants.Auth.ConsumerSecret
				},
				GoToTwitterAuthorization = DisplayPinPage,
				GetPin = GetPinFromUser
			};

			try
			{
				await auth.AuthorizeAsync().ConfigureAwait( false );
			}
			catch( TwitterQueryException ex )
			{
				if( !PinEntryCancelled )
				{
					LogTo.ErrorException( "Failed to authorize user", ex );
				}

				return;
			}

			var accountData = new TwitterAccountData
			{
				OAuthToken = auth.CredentialStore.OAuthToken,
				OAuthTokenSecret = auth.CredentialStore.OAuthTokenSecret,
				AccountName = auth.CredentialStore.ScreenName,
				UserId = auth.CredentialStore.UserID
			};

			if( ContextList.Contexts.All( c => c.UserId != accountData.UserId ) )
			{
				using( var ctx = new TwitterContext( auth ) )
				{
					var twitterUser =
						await
							ctx.User.Where( tw => tw.Type == UserType.Show && tw.UserID == accountData.UserId && tw.IncludeEntities == false )
								.SingleOrDefaultAsync();
					accountData.ImageUrl = twitterUser.ProfileImageUrlHttps.Replace( "_normal", "" );
				}

				ContextList.AddContext( accountData );

				var newColumns = await ViewServiceRepository.SelectAccountColumnTypes( accountData.UserId, DialogHostIdentifier );
				if( newColumns.Any() )
				{
					ColumnList.AddColumns( newColumns );
				}
			}

			Close( true );
		}

		private void ExecuteMakeDefaultAccountCommand( AccountEntry entry )
		{
			foreach( var acc in AddedAccounts )
			{
				acc.IsDefaultAccount = acc.Data.UserId == entry.Data.UserId;
			}

			ContextList.UpdateAllAccounts();
		}

		private string GetPinFromUser()
		{
			string input = ViewServiceRepository.TextInput( Strings.TwitterPinEntry, null, DialogHostIdentifier );
			if( string.IsNullOrWhiteSpace( input ) )
			{
				PinEntryCancelled = true;
			}
			return input;
		}

		public ICommand AddAccountCommand => _AddAccountCommand ?? ( _AddAccountCommand = new RelayCommand( ExecuteAddAccountCommand ) );
		public ICollection<AccountEntry> AddedAccounts { get; }

		public ICommand MakeDefaultAccountCommand => _MakeDefaultAccountCommand ?? ( _MakeDefaultAccountCommand = new RelayCommand<AccountEntry>(
			ExecuteMakeDefaultAccountCommand ) );

		private const string DialogHostIdentifier = "AccountDialogHost";
		private readonly IColumnDefinitionList ColumnList;
		private RelayCommand _AddAccountCommand;
		private RelayCommand<AccountEntry> _MakeDefaultAccountCommand;
		private bool PinEntryCancelled;
	}
}