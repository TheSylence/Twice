using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twice.Models.Columns;
using Twice.ViewModels.Flyouts;
using Twice.ViewModels.Twitter;

namespace Twice.Views.Services
{
	internal interface IViewServiceRepository
	{
		Task ComposeMessage();

		Task ComposeTweet( string text = null );

		Task<bool> Confirm( ConfirmServiceArgs args );

		Task<string> OpenFile( FileServiceArgs args = null );

		void OpenNotificationFlyout( NotificationViewModel vm );

		Task OpenSearch( string query = null );

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

		Task ViewImage( IList<StatusMediaViewModel> imageSet, StatusMediaViewModel selectedImage );

		Task ViewImage( IList<Uri> imageSet, Uri selectedImage );

		Task ViewProfile( ulong userId );

		Task ViewProfile( string screenName );

		Task ViewStatus( StatusViewModel vm );
	}
}