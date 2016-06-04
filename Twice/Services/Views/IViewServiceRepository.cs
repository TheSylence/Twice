using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twice.Models.Columns;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.Services.Views
{
	internal interface IViewServiceRepository
	{
		Task ComposeTweet();

		Task<bool> Confirm( ConfirmServiceArgs args );

		Task<string> OpenFile( FileServiceArgs args = null );

		Task QuoteTweet( StatusViewModel status, IEnumerable<ulong> preSelectedAccounts = null );

		Task ReplyToTweet( StatusViewModel status, bool toAll );

		Task RetweetStatus( StatusViewModel status );

		Task<ColumnDefinition[]> SelectAccountColumnTypes( ulong accountId );

		Task ShowAccounts( bool directlyAddNewAccount = false );

		Task ShowAddColumnDialog();

		Task ShowInfo();

		Task ShowSettings();

		string TextInput( string label, string input = null );

		Task ViewImage( IList<Uri> imageSet, Uri selectedImage );

		Task ViewProfile( ulong userId );

		Task ViewStatus( StatusViewModel vm, IContextEntry context );
	}
}