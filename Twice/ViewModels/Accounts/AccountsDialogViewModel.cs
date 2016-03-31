using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Anotar.NLog;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.Resources;
using Twice.ViewModels.Columns.Definitions;

namespace Twice.ViewModels.Accounts
{
	internal interface IAccountsDialogViewModel : IDialogViewModel
	{
		ICommand AddAccountCommand { get; }
		ICollection<AccountEntry> AddedAccounts { get; }
	}

	internal class AccountEntry
	{
		public AccountEntry( IContextEntry context )
		{
			AccountName = context.AccountName;
			ProfileImage = context.ProfileImageUrl;
			IsDefaultAccount = true;
		}

		public string AccountName { get; }
		public bool IsDefaultAccount { get; set; }
		public Uri ProfileImage { get; }
	}

	internal class AccountsDialogViewModel : DialogViewModel, IAccountsDialogViewModel
	{
		public AccountsDialogViewModel( IColumnDefinitionList columnList, ITwitterContextList contextList )
		{
			ContextList = contextList;
			ColumnList = columnList;

			AddedAccounts = ContextList.Contexts.Select( c => new AccountEntry( c ) ).ToList();
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
				await auth.AuthorizeAsync();
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

			using( var ctx = new TwitterContext( auth ) )
			{
				var twitterUser = await ctx.User.Where( tw => tw.Type == UserType.Show && tw.UserID == accountData.UserId && tw.IncludeEntities == false ).SingleOrDefaultAsync();
				accountData.ImageUrl = twitterUser.ProfileImageUrlHttps;
			}

			ContextList.AddContext( accountData );

			var newColumns = await ViewServiceRepository.SelectAccountColumnTypes( accountData.UserId , DialogHostIdentifier );
			if( newColumns.Any() )
			{
				var columns = ColumnList.Load();
				ColumnList.Save( columns.Concat( newColumns ) );
			}

			Close( true );
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

		private const string DialogHostIdentifier = "AccountDialogHost";
		private readonly IColumnDefinitionList ColumnList;
		private readonly ITwitterContextList ContextList;
		private RelayCommand _AddAccountCommand;
		private bool PinEntryCancelled;
	}
}