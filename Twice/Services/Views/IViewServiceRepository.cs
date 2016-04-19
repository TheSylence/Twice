using System.Threading.Tasks;
using Twice.Models.Columns;
using Twice.Views;

namespace Twice.Services.Views
{
	internal interface IViewServiceRepository
	{
		Task<bool> Confirm( ConfirmServiceArgs args );

		Task<string> OpenFile( FileServiceArgs args = null );

		Task<ColumnDefinition[]> SelectAccountColumnTypes( ulong accountId, string hostIdentifier );

		Task ShowAccounts( bool directlyAddNewAccount = false );

		Task ShowAddColumnDialog();

		Task ShowInfo();

		Task ShowSettings();

		string TextInput( string label, string input = null, string hostIdentifier = null );

		Task ViewProfile( ulong userId );

		Dialog CurrentDialog { get; }
	}
}