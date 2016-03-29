using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Anotar.NLog;
using GalaSoft.MvvmLight.CommandWpf;
using LinqToTwitter;
using Twice.Resources;
using Twice.ViewModels.Columns.Definitions;

namespace Twice.ViewModels.Accounts
{
	internal interface IAccountsDialogViewModel : IDialogViewModel
	{
		ICommand AddAccountCommand { get; }
	}

	internal class AccountsDialogViewModel : DialogViewModel, IAccountsDialogViewModel
	{
		public AccountsDialogViewModel( IColumnDefinitionList columnList )
		{
			ColumnList = columnList;
		}

		private readonly IColumnDefinitionList ColumnList;

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

			var newColumns = await ViewServiceRepository.SelectAccountColumnTypes( DialogHostIdentifier );
			if( newColumns.Any() )
			{
				var columns = ColumnList.Load();
				ColumnList.Save( columns.Concat( newColumns ) );
			}

			Close( true );
		}

		const string DialogHostIdentifier = "AccountDialogHost";

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
		private RelayCommand _AddAccountCommand;
		private bool PinEntryCancelled;
	}
}