using System.Threading.Tasks;
using Twice.ViewModels.Columns.Definitions;
using Twice.Views;

namespace Twice.Services.Views
{
	internal interface IViewServiceRepository
	{
		Task<bool> Confirm( ConfirmServiceArgs args );

		Task<string> OpenFile( FileServiceArgs args = null );

		Task ShowAccounts();

		Task ShowAddColumnDialog();

		Task ShowInfo();

		Task ShowSettings();

		Task<ColumnDefinition[]> SelectAccountColumnTypes( string hostIdentifier );

		string TextInput( string label, string input = null, string hostIdentifier = null );

		Task ViewProfile( ulong userId );

		Dialog CurrentDialog { get; }
	}
}