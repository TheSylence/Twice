using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twice.Models.Columns;
using Twice.ViewModels.Twitter;

namespace Twice.Services.Views
{
	internal interface IViewServiceRepository
	{
		Task ComposeMessage();

		Task ComposeTweet();

		Task<bool> Confirm( ConfirmServiceArgs args );

		Task<string> OpenFile( FileServiceArgs args = null );

		Task OpenSearch();

		Task QuoteTweet( StatusViewModel status, IEnumerable<ulong> preSelectedAccounts = null );

		Task ReplyToMessage( MessageViewModel vm );

		Task ReplyToTweet( StatusViewModel status, bool toAll );

		Task RetweetStatus( StatusViewModel status );

		Task<ColumnDefinition[]> SelectAccountColumnTypes( ulong accountId );

		Task ShowAccounts( bool directlyAddNewAccount = false );

		Task ShowAddColumnDialog();

		Task ShowInfo();

		Task ShowSettings();

		string TextInput( string label, string input = null );

		Task ViewDirectMessage( MessageViewModel vm );

		Task ViewImage( IList<Uri> imageSet, Uri selectedImage );

		Task ViewProfile( ulong userId );

		Task ViewStatus( StatusViewModel vm );
	}
}