using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.Resources;

namespace Twice.ViewModels.Accounts
{
	internal class AccountsDialogViewModel : DialogViewModel, IAccountsDialogViewModel
	{
		public AccountsDialogViewModel( IColumnDefinitionList columnList, ITwitterContextList contextList,
			ITwitterAuthorizer authorizer )
		{
			ContextList = contextList;
			ColumnList = columnList;
			Authorizer = authorizer;

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
			ProcessStarter.Start( url );
		}

		private async void ExecuteAddAccountCommand()
		{
			PinEntryCancelled = new CancellationTokenSource();

			var result = await Authorizer.Authorize( DisplayPinPage, GetPinFromUser, PinEntryCancelled.Token );
			var accountData = result.Data;
			if( accountData != null )
			{
				if( ContextList.Contexts.All( c => c.UserId != accountData.UserId ) )
				{
					using( var ctx = new TwitterContext( result.Auth ) )
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
				PinEntryCancelled.Cancel();
			}
			return input;
		}

		public ICommand AddAccountCommand
			=> _AddAccountCommand ?? ( _AddAccountCommand = new RelayCommand( ExecuteAddAccountCommand ) );

		public ICollection<AccountEntry> AddedAccounts { get; }

		public ICommand MakeDefaultAccountCommand
			=> _MakeDefaultAccountCommand ?? ( _MakeDefaultAccountCommand = new RelayCommand<AccountEntry>(
				ExecuteMakeDefaultAccountCommand ) );

		private const string DialogHostIdentifier = "AccountDialogHost";
		private readonly ITwitterAuthorizer Authorizer;
		private readonly IColumnDefinitionList ColumnList;
		private RelayCommand _AddAccountCommand;
		private RelayCommand<AccountEntry> _MakeDefaultAccountCommand;
		private CancellationTokenSource PinEntryCancelled;
	}
}